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
        ValidateRequest(request);

        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var locationTimeZoneId = await _locationReadOnlyRepository.GetActiveLocationTimeZoneIdById(request.LocationId, ct)
            ?? throw new NotFoundException("Location not found.");

        var timeZone = GetTimeZoneInfo(locationTimeZoneId);

        // 1. Entender qual "Dia" o cliente quer, na perspectiva do Fuso Horário do Local
        var localTargetDate = TimeZoneInfo.ConvertTimeFromUtc(request.TargetDate, timeZone).Date;
        var dayOfWeek = localTargetDate.DayOfWeek;

        // 2. Buscar as Grades de Trabalho (Schedules) para este dia da semana
        var schedules = await _scheduleReadRepository.GetSchedulesByDayAsync(
            request.ProfessionalId, request.LocationId, dayOfWeek, ct);

        // Se não trabalha nesse dia, retorna lista vazia na hora.
        if (schedules.Count == 0)
            return new ResponseAvailableTimeSlotsJson { AvailableSlotsUtc = [] };

        // 3. Determinar o Início e o Fim do Dia em UTC para buscar os agendamentos no banco
        var localStartOfDay = localTargetDate;
        var localEndOfDay = localTargetDate.AddDays(1);

        var utcStartOfDay = TimeZoneInfo.ConvertTimeToUtc(localStartOfDay, timeZone);
        var utcEndOfDay = TimeZoneInfo.ConvertTimeToUtc(localEndOfDay, timeZone);

        // 4. Buscar todos os agendamentos ocupados neste dia
        var appointments = await _appointmentReadRepository.GetAppointmentsByDateRangeAsync(
            request.ProfessionalId, request.LocationId, utcStartOfDay, utcEndOfDay, ct);

        // 5. O ALGORITMO: Fatiar a Grade e verificar disponibilidade
        var availableSlotsUtc = new List<DateTime>();
        var serviceDuration = TimeSpan.FromMinutes(service.DurationInMinutes);
        var nowUtc = DateTime.UtcNow;

        foreach (var schedule in schedules)
        {
            var currentSlotLocalTime = schedule.StartTime;
            var scheduleEndLocalTime = schedule.EndTime;

            // Continua fatiando enquanto o serviço couber dentro do turno
            while (currentSlotLocalTime.Add(serviceDuration) <= scheduleEndLocalTime)
            {
                // Monta o DateTime exato do Slot no fuso horário local
                var slotLocalStartDateTime = localTargetDate.Add(currentSlotLocalTime);
                var slotLocalEndDateTime = slotLocalStartDateTime.Add(serviceDuration);

                // Converte para UTC para fazer as comparações
                var slotUtcStart = TimeZoneInfo.ConvertTimeToUtc(slotLocalStartDateTime, timeZone);
                var slotUtcEnd = TimeZoneInfo.ConvertTimeToUtc(slotLocalEndDateTime, timeZone);

                // REGRA 1: O horário já passou? (Para agendamentos no mesmo dia)
                if (slotUtcStart > nowUtc)
                {
                    // REGRA 2: Esse slot bate em algum agendamento existente?
                    // Lógica de sobreposição: (Início Slot < Fim Agendamento) E (Fim Slot > Início Agendamento)
                    var hasOverlap = appointments.Any(a =>
                        slotUtcStart < a.EndTime &&
                        slotUtcEnd > a.StartTime);

                    if (!hasOverlap)
                    {
                        availableSlotsUtc.Add(slotUtcStart);
                    }
                }

                // Pula para o próximo horário (o "Passo" é a própria duração do serviço)
                currentSlotLocalTime = currentSlotLocalTime.Add(serviceDuration);
            }
        }

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
