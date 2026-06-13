using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.ScheduleBlocks;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.ScheduleBlocks;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public class RegisterScheduleBlockUseCase : IRegisterScheduleBlockUseCase
{
    private readonly IScheduleBlockWriteOnlyRepository _writeRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterScheduleBlockUseCase(
        IScheduleBlockWriteOnlyRepository writeRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _userRepository = userRepository;
        _locationReadRepository = locationReadRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterScheduleBlockJson> ExecuteAsync(RequestRegisterScheduleBlockJson request, CancellationToken ct = default)
    {
        request.Reason = request.Reason?.Trim();

        await ValidateAsync(request, ct);

        var block = new ScheduleBlock(
            request.ProfessionalId,
            request.LocationId,
            request.StartTime,
            request.EndTime,
            request.Reason);

        await _writeRepository.AddAsync(block, ct);
        await _unitOfWork.Commit(ct);

        return new ResponseRegisterScheduleBlockJson { Id = block.Id };
    }

    private async Task ValidateAsync(RequestRegisterScheduleBlockJson request, CancellationToken ct)
    {
        var validator = new RegisterScheduleBlockValidator();
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
            throw new ConflictException("This schedule block overlaps with existing active appointments. Cancel or reschedule them before creating the block.");
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
