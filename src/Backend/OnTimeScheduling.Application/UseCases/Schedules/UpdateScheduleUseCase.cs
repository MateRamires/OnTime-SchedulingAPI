using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Schedules;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class UpdateScheduleUseCase : IUpdateScheduleUseCase
{
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly IProfessionalScheduleWriteOnlyRepository _writeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly FutureAppointmentScheduleGuard _futureAppointmentScheduleGuard;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleUseCase(
        IProfessionalScheduleReadOnlyRepository readRepository,
        IProfessionalScheduleWriteOnlyRepository writeRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository,
        ITenantProvider tenantProvider,
        FutureAppointmentScheduleGuard futureAppointmentScheduleGuard,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _userRepository = userRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
        _tenantProvider = tenantProvider;
        _futureAppointmentScheduleGuard = futureAppointmentScheduleGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, RequestUpdateScheduleJson request, CancellationToken ct = default)
    {
        var schedule = await _readRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Professional schedule not found.");

        await Validate(request, id, ct);

        await _futureAppointmentScheduleGuard.EnsureCanUpdateAsync(schedule, request, ct);

        schedule.Update(
            request.UserId,
            request.LocationId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime);

        _writeRepository.Update(schedule);
        await _unitOfWork.Commit(ct);
    }

    private async Task Validate(RequestUpdateScheduleJson request, Guid scheduleId, CancellationToken ct)
    {
        var validator = new UpdateScheduleValidator();
        var result = validator.Validate(request);

        if (!_tenantProvider.CompanyId.HasValue)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));
        }
        else
        {
            var companyId = _tenantProvider.CompanyId.Value;

            var professional = await _userRepository.GetByIdAndCompany(request.UserId, companyId, ct);
            if (professional is null)
                result.Errors.Add(new ValidationFailure(nameof(request.UserId), "Professional not found in this tenant."));
            else if (professional.Role != UserRole.PROVIDER)
                result.Errors.Add(new ValidationFailure(nameof(request.UserId), "Only provider users can have schedules."));

            var locationExists = await _locationReadOnlyRepository.ExistsActiveLocationById(request.LocationId, ct);
            if (!locationExists)
                result.Errors.Add(new ValidationFailure(nameof(request.LocationId), "Location not found in this tenant."));
        }

        var hasOverlap = await _readRepository.HasOverlappingSchedule(
            request.UserId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            ct,
            scheduleId);

        if (hasOverlap)
        {
            result.Errors.Add(new ValidationFailure(
                string.Empty,
                "This schedule overlaps with an existing schedule for this professional on the selected day."));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
