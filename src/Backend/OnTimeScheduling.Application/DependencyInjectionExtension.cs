using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.UseCases.Appointments;
using OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;
using OnTimeScheduling.Application.UseCases.Clients;
using OnTimeScheduling.Application.UseCases.Companies;
using OnTimeScheduling.Application.UseCases.Locations;
using OnTimeScheduling.Application.UseCases.Reports;
using OnTimeScheduling.Application.UseCases.ScheduleBlocks;
using OnTimeScheduling.Application.UseCases.Schedules;
using OnTimeScheduling.Application.UseCases.Services;
using OnTimeScheduling.Application.UseCases.Users.Auth;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using OnTimeScheduling.Application.UseCases.Users.Login;
using OnTimeScheduling.Application.UseCases.Users.Management;

namespace OnTimeScheduling.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
        services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IActivateUserUseCase, ActivateUserUseCase>();
        services.AddScoped<IInactivateUserUseCase, InactivateUserUseCase>();
        services.AddScoped<IRegisterSuperAdminUseCase, RegisterSuperAdminUseCase>();

        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<ILogoutUseCase, LogoutUseCase>();
        services.AddScoped<IGetCurrentUserUseCase, GetCurrentUserUseCase>();

        services.AddScoped<IRegisterCompanyUseCase, RegisterCompanyUseCase>();
        services.AddScoped<IGetCompaniesUseCase, GetCompaniesUseCase>();
        services.AddScoped<IGetCompanyByIdUseCase, GetCompanyByIdUseCase>();
        services.AddScoped<IUpdateCompanyUseCase, UpdateCompanyUseCase>();
        services.AddScoped<IActivateCompanyUseCase, ActivateCompanyUseCase>();
        services.AddScoped<IInactivateCompanyUseCase, InactivateCompanyUseCase>();
        services.AddScoped<IRegisterCompanyAdminUseCase, RegisterCompanyAdminUseCase>();

        services.AddScoped<IRegisterLocationUseCase, RegisterLocationUseCase>();
        services.AddScoped<IGetLocationsUseCase, GetLocationsUseCase>();
        services.AddScoped<IGetLocationByIdUseCase, GetLocationByIdUseCase>();
        services.AddScoped<IUpdateLocationUseCase, UpdateLocationUseCase>();
        services.AddScoped<IActivateLocationUseCase, ActivateLocationUseCase>();
        services.AddScoped<IInactivateLocationUseCase, InactivateLocationUseCase>();

        services.AddScoped<IRegisterServiceUseCase, RegisterServiceUseCase>();

        services.AddScoped<ILinkProfessionalServiceUseCase, LinkProfessionalServiceUseCase>();

        services.AddScoped<IGetServicesUseCase, GetServicesUseCase>();
        services.AddScoped<IGetServiceByIdUseCase, GetServiceByIdUseCase>();
        services.AddScoped<IUpdateServiceUseCase, UpdateServiceUseCase>();
        services.AddScoped<IActivateServiceUseCase, ActivateServiceUseCase>();
        services.AddScoped<IInactivateServiceUseCase, InactivateServiceUseCase>();
        services.AddScoped<IUnlinkProfessionalServiceUseCase, UnlinkProfessionalServiceUseCase>();

        services.AddScoped<IRegisterScheduleUseCase, RegisterScheduleUseCase>();
        services.AddScoped<IGetProfessionalSchedulesUseCase, GetProfessionalSchedulesUseCase>();
        services.AddScoped<IGetProfessionalScheduleByIdUseCase, GetProfessionalScheduleByIdUseCase>();
        services.AddScoped<IUpdateScheduleUseCase, UpdateScheduleUseCase>();
        services.AddScoped<IDeleteScheduleUseCase, DeleteScheduleUseCase>();
        services.AddScoped<FutureAppointmentScheduleGuard>();

        services.AddScoped<IRegisterScheduleBlockUseCase, RegisterScheduleBlockUseCase>();
        services.AddScoped<IGetScheduleBlocksUseCase, GetScheduleBlocksUseCase>();
        services.AddScoped<IGetScheduleBlockByIdUseCase, GetScheduleBlockByIdUseCase>();
        services.AddScoped<IUpdateScheduleBlockUseCase, UpdateScheduleBlockUseCase>();
        services.AddScoped<IDeleteScheduleBlockUseCase, DeleteScheduleBlockUseCase>();

        services.AddScoped<IRegisterAppointmentUseCase, RegisterAppointmentUseCase>();
        services.AddScoped<IGetAppointmentsUseCase, GetAppointmentsUseCase>();
        services.AddScoped<IGetAppointmentByIdUseCase, GetAppointmentByIdUseCase>();
        services.AddScoped<IGetAvailableTimeSlotsUseCase, GetAvailableTimeSlotsUseCase>();
        services.AddScoped<ICancelAppointmentUseCase, CancelAppointmentUseCase>();
        services.AddScoped<IUpdateAppointmentUseCase, UpdateAppointmentUseCase>();
        services.AddScoped<IUpdateAppointmentStatusUseCase, UpdateAppointmentStatusUseCase>();
        services.AddScoped<IGetDailyAgendaUseCase, GetDailyAgendaUseCase>();
        services.AddScoped<IGetMyAgendaUseCase, GetMyAgendaUseCase>();

        services.AddScoped<IGetAppointmentsVolumeReportUseCase, GetAppointmentsVolumeReportUseCase>();
        services.AddScoped<IGetProfessionalOccupancyReportUseCase, GetProfessionalOccupancyReportUseCase>();

        services.AddScoped<IRegisterClientUseCase, RegisterClientUseCase>();
        services.AddScoped<IGetClientsUseCase, GetClientsUseCase>();
        services.AddScoped<IGetClientByIdUseCase, GetClientByIdUseCase>();
        services.AddScoped<IUpdateClientUseCase, UpdateClientUseCase>();
        services.AddScoped<IActivateClientUseCase, ActivateClientUseCase>();
        services.AddScoped<IInactivateClientUseCase, InactivateClientUseCase>();
        services.AddScoped<IDeleteClientUseCase, DeleteClientUseCase>();
    }
}
