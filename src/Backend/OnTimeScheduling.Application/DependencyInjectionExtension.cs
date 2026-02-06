using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.UseCases.Companies;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using OnTimeScheduling.Application.UseCases.Users.Login;

namespace OnTimeScheduling.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services) 
    {
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterCompanyUseCase, RegisterCompanyUseCase>();
    }
}
