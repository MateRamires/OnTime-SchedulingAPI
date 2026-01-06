using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using System.Runtime.CompilerServices;

namespace OnTimeScheduling.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services) 
    {
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
    }
}
