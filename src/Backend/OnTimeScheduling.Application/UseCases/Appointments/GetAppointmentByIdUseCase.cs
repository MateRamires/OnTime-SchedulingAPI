using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Appointments.Mapper;
using OnTimeScheduling.Communication.Responses.Appointments;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class GetAppointmentByIdUseCase : IGetAppointmentByIdUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetAppointmentByIdUseCase(IAppointmentReadOnlyRepository appointmentReadRepository, ITenantProvider tenantProvider)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseAppointmentJson> ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var appointment = await _appointmentReadRepository.GetAppointmentDetailsByIdAsync(appointmentId, ct)
            ?? throw new NotFoundException("Appointment not found.");

        return AppointmentResponseMapper.MapDetails(appointment);
    }

}
