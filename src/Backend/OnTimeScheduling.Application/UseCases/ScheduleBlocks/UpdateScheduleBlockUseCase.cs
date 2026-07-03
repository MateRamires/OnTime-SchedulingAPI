using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.ScheduleBlocks;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public class UpdateScheduleBlockUseCase : IUpdateScheduleBlockUseCase
{
    private readonly IScheduleBlockReadOnlyRepository _readRepository;
    private readonly IScheduleBlockWriteOnlyRepository _writeRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly IAgendaConcurrencyGuard _agendaConcurrencyGuard;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleBlockUseCase(
        IScheduleBlockReadOnlyRepository readRepository,
        IScheduleBlockWriteOnlyRepository writeRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadRepository,
        IAgendaConcurrencyGuard agendaConcurrencyGuard,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _userRepository = userRepository;
        _locationReadRepository = locationReadRepository;
        _agendaConcurrencyGuard = agendaConcurrencyGuard;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, RequestUpdateScheduleBlockJson request, CancellationToken ct = default)
    {
        request.Reason = request.Reason?.Trim();

        await _agendaConcurrencyGuard.ExecuteAsync(
            CreateLockKeys(request.ProfessionalId, request.LocationId),
            async lockedCt =>
            {
                var block = await _readRepository.GetByIdAsync(id, lockedCt)
                    ?? throw new NotFoundException("Schedule block not found.");

                await ValidateAsync(request, lockedCt);

                block.Update(
                    request.ProfessionalId,
                    request.LocationId,
                    request.StartTime,
                    request.EndTime,
                    request.Reason);

                _writeRepository.Update(block);
                await _unitOfWork.Commit(lockedCt);
            },
            ct);
    }

    private static List<AgendaConcurrencyLockKey> CreateLockKeys(Guid? professionalId, Guid? locationId)
    {
        var lockKeys = new List<AgendaConcurrencyLockKey>();

        if (professionalId.HasValue)
            lockKeys.Add(AgendaConcurrencyLockKey.ForProfessional(professionalId.Value));

        if (locationId.HasValue)
            lockKeys.Add(AgendaConcurrencyLockKey.ForLocation(locationId.Value));

        return lockKeys;
    }

    private async Task ValidateAsync(RequestUpdateScheduleBlockJson request, CancellationToken ct)
    {
        var validator = new UpdateScheduleBlockValidator();
        var result = validator.Validate(request);

        await AddBusinessValidationErrorsAsync(request.ProfessionalId, request.LocationId, result, ct);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }

        var overlapsExistingAppointment = await _appointmentReadRepository.HasOverlappingAppointmentForBlockAsync(
            request.ProfessionalId,
            request.LocationId,
            request.StartTime,
            request.EndTime,
            ct);

        if (overlapsExistingAppointment)
            throw new ConflictException("This schedule block overlaps with existing active appointments. Cancel or reschedule them before updating the block.");
    }

    private async Task AddBusinessValidationErrorsAsync(
        Guid? professionalId,
        Guid? locationId,
        ValidationResult result,
        CancellationToken ct)
    {
        var companyId = _tenantProvider.CompanyId;
        if (!companyId.HasValue)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));
            return;
        }

        if (professionalId.HasValue)
        {
            var professional = await _userRepository.GetByIdAndCompany(professionalId.Value, companyId.Value, ct);
            if (professional is null)
                result.Errors.Add(new ValidationFailure(nameof(professionalId), "Professional not found in this tenant."));
            else if (professional.Role != UserRole.PROVIDER)
                result.Errors.Add(new ValidationFailure(nameof(professionalId), "Only provider users can be blocked by schedule blocks."));
        }

        if (locationId.HasValue)
        {
            var locationExists = await _locationReadRepository.ExistsActiveLocationById(locationId.Value, ct);
            if (!locationExists)
                result.Errors.Add(new ValidationFailure(nameof(locationId), "Location not found in this tenant."));
        }
    }

}
