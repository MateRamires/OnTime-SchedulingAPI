using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Schedules;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Schedules;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class RegisterScheduleUseCase : IRegisterScheduleUseCase
{
    private readonly IProfessionalScheduleWriteOnlyRepository _writeRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterScheduleUseCase(
        IProfessionalScheduleWriteOnlyRepository writeRepository,
        IProfessionalScheduleReadOnlyRepository readRepository,
        IUserRepository userRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _userRepository = userRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterScheduleJson> ExecuteAsync(RequestRegisterScheduleJson request, CancellationToken ct = default)
    {
        await Validate(request, ct);

        var schedule = new ProfessionalSchedule(
            userId: request.UserId,
            locationId: request.LocationId,
            dayOfWeek: request.DayOfWeek,
            startTime: request.StartTime,
            endTime: request.EndTime
        );

        await _writeRepository.Add(schedule, ct);
        await _unitOfWork.Commit(ct);

        return new ResponseRegisterScheduleJson
        {
            Id = schedule.Id
        };
    }

    private async Task Validate(RequestRegisterScheduleJson request, CancellationToken ct = default)
    {
        var validator = new RegisterScheduleValidator();
        var result = validator.Validate(request);

        if (!_tenantProvider.CompanyId.HasValue)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));

        if (_tenantProvider.CompanyId.HasValue)
        {
            var companyId = _tenantProvider.CompanyId.Value;

            var professional = await _userRepository.GetByIdAndCompany(request.UserId, companyId, ct);
            if (professional is null)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Professional not found in this tenant."));
            else if (professional.Role != UserRole.PROVIDER)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Only provider users can have schedules."));
            

            var locationExists = await _locationReadOnlyRepository.ExistsActiveLocationById(request.LocationId, ct);
            if (!locationExists)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Location not found in this tenant."));
        }


        var hasOverlap = await _readRepository.HasOverlappingSchedule(
            request.UserId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            ct);

        if (hasOverlap)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                string.Empty,
                "This schedule block overlaps with an existing schedule for this professional on the selected day."));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
