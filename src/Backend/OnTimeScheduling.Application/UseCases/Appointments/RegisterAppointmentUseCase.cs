using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class RegisterAppointmentUseCase : IRegisterAppointmentUseCase
{
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAppointmentUseCase(
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        IProfessionalServiceReadOnlyRepository professionalServiceReadRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentWriteRepository = appointmentWriteRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _serviceReadRepository = serviceReadRepository;
        _professionalServiceReadRepository = professionalServiceReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterAppointmentJson> ExecuteAsync(RequestRegisterAppointmentJson request, CancellationToken ct = default)
    {
        ValidateBasicFields(request);

        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var endTime = request.StartTime.AddMinutes(service.DurationInMinutes);

        await ValidateBusinessRulesAsync(request, endTime, ct);

        var appointment = new Appointment(
            professionalId: request.ProfessionalId,
            serviceId: request.ServiceId,
            locationId: request.LocationId,
            clientName: request.ClientName,
            clientPhone: request.ClientPhone,
            startTime: request.StartTime,
            endTime: endTime 
        );

        await _appointmentWriteRepository.Add(appointment, ct);
        await _unitOfWork.Commit();

        return new ResponseRegisterAppointmentJson
        {
            Id = appointment.Id
        };
    }

    private void ValidateBasicFields(RequestRegisterAppointmentJson request)
    {
        var validator = new RegisterAppointmentValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private async Task ValidateBusinessRulesAsync(RequestRegisterAppointmentJson request, DateTime calculatedEndTime, CancellationToken ct)
    {
        // Regra A: O profissional realiza este serviço?
        var doesProfessionalPerformService = await _professionalServiceReadRepository
            .Exists(request.ProfessionalId, request.ServiceId, ct);

        if (!doesProfessionalPerformService)
            throw new ErrorOnValidationException(["This professional does not provide the selected service."]);

        // Regra B: O horário está livre? (Evitar sobreposição)
        var isTimeSlotTaken = await _appointmentReadRepository
            .HasOverlappingAppointment(request.ProfessionalId, request.StartTime, calculatedEndTime, ct);

        if (isTimeSlotTaken)
            throw new ErrorOnValidationException(["The selected time slot is no longer available."]);

        // NOTA DE ARQUITETURA: 
        // Em um cenário 100% completo, aqui também verificaríamos se o request.StartTime 
        // está dentro da "ProfessionalSchedule" (Grade Regular). Mas geralmente, delegamos 
        // essa responsabilidade para a consulta de "Horários Disponíveis" que o Front-end chama antes de agendar.
    }
}
