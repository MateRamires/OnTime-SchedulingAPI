using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnTimeScheduling.Api.ErrorHandling;
using OnTimeScheduling.Api.RateLimiting;
using OnTimeScheduling.Application;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Infrastructure;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "frontend";

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    });

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = ApiProblemDetails.CreateValidation(context.HttpContext, context.ModelState);
        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "OnTime Scheduling API", Version = "v1" });
    config.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcd'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey");

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
    };

    config.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            var problem = ApiProblemDetails.Create(
                context.HttpContext,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The access token is missing, expired, or invalid.");

            await ApiProblemDetails.WriteAsync(context.HttpContext, problem);
        },
        OnForbidden = async context =>
        {
            var problem = ApiProblemDetails.Create(
                context.HttpContext,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "The authenticated user does not have permission to perform this operation.");

            await ApiProblemDetails.WriteAsync(context.HttpContext, problem);
        }
    };
});

builder.Services.AddApiRateLimiting(builder.Configuration);

//Add Application's Dependency Injections
builder.Services.AddApplication();

//Add Infrastructure's Dependency Injections
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await MigrateDatabase(app);

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    var (title, detail) = httpContext.Response.StatusCode switch
    {
        StatusCodes.Status404NotFound => ("Not Found", "The requested endpoint was not found."),
        StatusCodes.Status405MethodNotAllowed => ("Method Not Allowed", "The HTTP method is not supported by this endpoint."),
        _ => (ReasonPhrases.GetReasonPhrase(httpContext.Response.StatusCode), "The request could not be completed.")
    };

    var problem = ApiProblemDetails.Create(
        httpContext,
        httpContext.Response.StatusCode,
        title,
        detail);

    await ApiProblemDetails.WriteAsync(httpContext, problem);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();




async Task MigrateDatabase(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHashService>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting database migration and seeding...");

        await context.Database.MigrateAsync();

        await DbInitializer.Seed(context, passwordHasher, configuration);

        logger.LogInformation("Database is ready for use.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"CRITICAL ERROR DURING STARTUP: {ex.Message}");
        throw;
    }
}
