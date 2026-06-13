using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Locations;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class UpdateLocationUseCase : IUpdateLocationUseCase
{
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ILocationWriteOnlyRepository _locationWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLocationUseCase(
        ILocationReadOnlyRepository locationReadRepository,
        ILocationWriteOnlyRepository locationWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid locationId, RequestUpdateLocationJson request, CancellationToken ct = default)
    {
        request.Name = request.Name.FormatName();
        request.Address = request.Address?.Trim() ?? string.Empty;
        request.TimeZoneId = string.IsNullOrWhiteSpace(request.TimeZoneId) ? null : request.TimeZoneId.Trim();

        await Validate(locationId, request, ct);

        var location = await _locationReadRepository.GetByIdAsync(locationId, ct)
            ?? throw new NotFoundException("Location not found.");

        location.Update(request.Name, request.Address, request.TimeZoneId);

        _locationWriteRepository.Update(location);
        await _unitOfWork.Commit(ct);
    }

    private async Task Validate(Guid locationId, RequestUpdateLocationJson request, CancellationToken ct)
    {
        var validator = new UpdateLocationValidator();
        var result = validator.Validate(request);

        var currentCompanyId = _tenantProvider.CompanyId;
        if (!currentCompanyId.HasValue)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));

        if (currentCompanyId.HasValue)
        {
            var nameExists = await _locationReadRepository.ExistsLocationWithNameExceptId(request.Name, locationId, currentCompanyId.Value, ct);
            if (nameExists)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.Name), "A location with this name already exists in your company."));
        }

        ValidateTimeZone(request.TimeZoneId, result);

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }

    private static void ValidateTimeZone(string? timeZoneId, FluentValidation.Results.ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
            return;

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Trim());
        }
        catch (TimeZoneNotFoundException)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(timeZoneId), "The provided timezone is invalid."));
        }
        catch (InvalidTimeZoneException)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(timeZoneId), "The provided timezone is invalid."));
        }
    }

}
