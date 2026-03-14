using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class RegisterAppointmentUseCase : IRegisterAppointmentUseCase
{
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _scheduleReadRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAppointmentUseCase(
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        IProfessionalServiceReadOnlyRepository professionalServiceReadRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository,
        IProfessionalScheduleReadOnlyRepository scheduleReadRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _appointmentWriteRepository = appointmentWriteRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _serviceReadRepository = serviceReadRepository;
        _professionalServiceReadRepository = professionalServiceReadRepository;
        _userRepository = userRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterAppointmentJson> ExecuteAsync(RequestRegisterAppointmentJson request, CancellationToken ct = default)
    {
        //TODO: verificar necessidade de separar Validates
        ValidateBasicFields(request);

        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var sanitizedClientName = request.ClientName.Trim();
        var sanitizedClientPhone = request.ClientPhone.Trim();

        var startTimeUtc = request.StartTime;
        var endTime = startTimeUtc.AddMinutes(service.DurationInMinutes);

        await ValidateBusinessRulesAsync(request, startTimeUtc, endTime, ct);

        var appointment = new Appointment(
            professionalId: request.ProfessionalId,
            serviceId: request.ServiceId,
            locationId: request.LocationId,
            clientName: sanitizedClientName,
            clientPhone: sanitizedClientPhone,
            startTime: startTimeUtc,
            endTime: endTime 
        );

        await _appointmentWriteRepository.Add(appointment, ct);
        await _unitOfWork.Commit(ct);

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

    private async Task ValidateBusinessRulesAsync(RequestRegisterAppointmentJson request,DateTime startTimeUtc,DateTime calculatedEndTime, CancellationToken ct)
    {
        var errors = new List<string>();

        if (!_tenantProvider.CompanyId.HasValue)
            errors.Add("The authenticated user does not have a valid tenant context.");

        if (_tenantProvider.CompanyId.HasValue)
        {
            var professional = await _userRepository.GetByIdAndCompany(request.ProfessionalId, _tenantProvider.CompanyId.Value, ct);
            if (professional is null)
                errors.Add("Professional not found in this tenant.");
            else if (professional.Role != UserRole.PROVIDER)
                errors.Add("Only provider users can receive appointments.");
        }

        var locationTimeZoneId = await _locationReadOnlyRepository.GetActiveLocationTimeZoneIdById(request.LocationId, ct);
        if (locationTimeZoneId is null)
            errors.Add("Location not found in this tenant.");

        var doesProfessionalPerformService = await _professionalServiceReadRepository
            .Exists(request.ProfessionalId, request.ServiceId, ct);

        if (!doesProfessionalPerformService)
            errors.Add("This professional does not provide the selected service.");

        var isTimeSlotTaken = await _appointmentReadRepository
            .HasOverlappingAppointment(request.ProfessionalId, startTimeUtc, calculatedEndTime, ct);

        if (isTimeSlotTaken)
            errors.Add("The selected time slot is no longer available.");

        if (locationTimeZoneId is not null)
        {
            TimeZoneInfo? locationTimeZone = null;

            try
            {
                locationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(locationTimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                errors.Add("Location timezone is invalid.");
            }
            catch (InvalidTimeZoneException)
            {
                errors.Add("Location timezone is invalid.");
            }

            if (locationTimeZone is not null)
            {
                var localStartTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeUtc, locationTimeZone);
                var localEndTime = TimeZoneInfo.ConvertTimeFromUtc(calculatedEndTime, locationTimeZone);

                var localDayOfWeek = localStartTime.DayOfWeek;
                var localAppointmentStart = localStartTime.TimeOfDay;
                var localAppointmentEnd = localEndTime.TimeOfDay;
                var spansDifferentLocalDays = localEndTime.Date != localStartTime.Date;

                if (spansDifferentLocalDays)
                {
                    errors.Add("Appointments cannot span across different days.");
                }
                else
                {
                    var isInsideProfessionalSchedule = await _scheduleReadRepository.HasCoverageForAppointment(
                        request.ProfessionalId,
                        request.LocationId,
                        localDayOfWeek,
                        localAppointmentStart,
                        localAppointmentEnd,
                        ct);

                    if (!isInsideProfessionalSchedule)
                        errors.Add("The selected time slot is outside the professional's regular schedule for this location.");
                }
            }
        }



        if (errors.Count != 0)
            throw new ErrorOnValidationException(errors);
    }
}
