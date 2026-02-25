using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnTimeScheduling.Api.Filters;
using OnTimeScheduling.Application;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Infrastructure;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "On Time Scheduling API", Version = "v1" });

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
        OnChallenge = context =>
        {
            context.HandleResponse(); 
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

            var response = new ResponseErrorJson("Your session has expired or is invalid. Please login again.", traceId);

            return context.Response.WriteAsJsonAsync(response);
        }
    };
});

//Add Application's Dependency Injections
builder.Services.AddApplication();

//Add Infrastructure's Dependency Injections
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await MigrateDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
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
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting database migration and seeding...");

        await context.Database.MigrateAsync();

        await DbInitializer.Seed(context, passwordHasher);

        logger.LogInformation("Database is ready for use.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"CRITICAL ERROR DURING STARTUP: {ex.Message}");
        throw;
    }
}