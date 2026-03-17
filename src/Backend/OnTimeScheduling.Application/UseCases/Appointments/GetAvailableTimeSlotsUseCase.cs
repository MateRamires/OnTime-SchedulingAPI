using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class GetAvailableTimeSlotsUseCase : IGetAvailableTimeSlotsUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _scheduleReadRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

    public GetAvailableTimeSlotsUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IProfessionalScheduleReadOnlyRepository scheduleReadRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _serviceReadRepository = serviceReadRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
    }

    public async Task<ResponseAvailableTimeSlotsJson> ExecuteAsync(RequestGetAvailableTimeSlotsJson request, CancellationToken ct = default)
    {
        // 1. Validar a Request Básica
        ValidateRequest(request);

        // 2. Buscar informações essenciais (Serviço e Fuso Horário do Local)
        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var locationTimeZoneId = await _locationReadOnlyRepository.GetActiveLocationTimeZoneIdById(request.LocationId, ct)
            ?? throw new NotFoundException("Location not found.");

        var timeZone = GetTimeZoneInfo(locationTimeZoneId);

        // 3. O Algoritmo de Disponibilidade (A ser implementado na Parte 2)
        // Aqui precisaremos:
        // a) Pegar a TargetDate do request (que pode vir em UTC) e converter para a Data Local do Local de Atendimento.
        // b) Buscar os "ProfessionalSchedules" desse dia da semana (pode haver mais de um turno no dia, ex: 08-12 e 13-18).
        // c) Buscar os "Appointments" que caem nessa data local.
        // d) Interpolar os turnos com os agendamentos e fatiar de acordo com `service.DurationInMinutes`.

        var availableSlotsUtc = new List<DateTime>();

        return new ResponseAvailableTimeSlotsJson
        {
            AvailableSlotsUtc = availableSlotsUtc
        };
    }

    private void ValidateRequest(RequestGetAvailableTimeSlotsJson request)
    {
        var validator = new GetAvailableTimeSlotsValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }

    private TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
        {
            throw new ErrorOnValidationException(["Invalid location time zone configuration."]);
        }
    }
}
