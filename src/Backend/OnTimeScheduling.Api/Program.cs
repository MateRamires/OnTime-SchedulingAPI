using OnTimeScheduling.Api.Filters;
using OnTimeScheduling.Application;
using OnTimeScheduling.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Application's Dependency Injections
builder.Services.AddApplication();

//Add Infrastructure's Dependency Injections
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
