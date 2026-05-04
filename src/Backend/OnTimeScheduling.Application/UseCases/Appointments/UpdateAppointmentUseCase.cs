
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class UpdateAppointmentUseCase : IUpdateAppointmentUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _scheduleReadRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        IClientReadOnlyRepository clientReadRepository,
        IProfessionalServiceReadOnlyRepository professionalServiceReadRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository,
        IProfessionalScheduleReadOnlyRepository scheduleReadRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _appointmentWriteRepository = appointmentWriteRepository;
        _serviceReadRepository = serviceReadRepository;
        _professionalServiceReadRepository = professionalServiceReadRepository;
        _clientReadRepository = clientReadRepository;
        _userRepository = userRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid appointmentId, RequestUpdateAppointmentJson request, CancellationToken ct = default)
    {
        ValidateBasicFields(request);

        var appointment = await _appointmentReadRepository.GetAppointmentByIdAsync(appointmentId, ct)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new ErrorOnValidationException(["Only scheduled appointments can be edited."]);

        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var startTimeUtc = request.StartTime;
        var endTimeUtc = startTimeUtc.AddMinutes(service.DurationInMinutes);

        await ValidateBusinessRulesAsync(appointmentId, request, startTimeUtc, endTimeUtc, ct);

        appointment.Reschedule(
            request.ClientId,
            request.ProfessionalId,
            request.ServiceId,
            request.LocationId,
            startTimeUtc,
            endTimeUtc);

        _appointmentWriteRepository.Update(appointment);
        await _unitOfWork.Commit(ct);
    }

    private void ValidateBasicFields(RequestUpdateAppointmentJson request)
    {
        var validator = new UpdateAppointmentValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private async Task ValidateBusinessRulesAsync(
        Guid appointmentId,
        RequestUpdateAppointmentJson request,
        DateTime startTimeUtc,
        DateTime calculatedEndTime,
        CancellationToken ct)
    {
        var errors = new List<string>();

        if (!_tenantProvider.CompanyId.HasValue)
            errors.Add("The authenticated user does not have a valid tenant context.");

        if (_tenantProvider.CompanyId.HasValue)
        {
            var client = await _clientReadRepository.GetByIdAsync(request.ClientId, ct);
            if (client is null)
                errors.Add("Client not found in this tenant.");

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
            .HasOverlappingAppointment(request.ProfessionalId, startTimeUtc, calculatedEndTime, ct, appointmentId);

        if (isTimeSlotTaken)
            throw new ConflictException("The selected time slot is no longer available due to an overlapping appointment.");

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
