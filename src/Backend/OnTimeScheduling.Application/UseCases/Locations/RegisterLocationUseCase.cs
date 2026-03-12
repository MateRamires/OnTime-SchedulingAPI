using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Locations;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Locations;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class RegisterLocationUseCase : IRegisterLocationUseCase
{
    private readonly ILocationWriteOnlyRepository _repository;
    private readonly ILocationReadOnlyRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;
    public RegisterLocationUseCase(ILocationWriteOnlyRepository repository, ILocationReadOnlyRepository readRepository, IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _repository = repository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }
    public async Task<ResponseRegisterLocationJson> ExecuteAsync(RequestRegisterLocationJson request, CancellationToken ct)
    {
        request.Name = request.Name.FormatName();
        request.Address = request.Address?.Trim() ?? string.Empty;

        await Validate(request, ct);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var location = new Location(
            companyId: companyId,
            name: request.Name,
            address: request.Address,
            timeZoneId: request.TimeZoneId
        );

        await _repository.Add(location, ct);
        await _unitOfWork.Commit();

        return new ResponseRegisterLocationJson
        {
            Id = location.Id,
            Name = location.Name
        };
    }

    private async Task Validate(RequestRegisterLocationJson request, CancellationToken ct)
    {
        var validator = new RegisterLocationValidator();
        var result = validator.Validate(request);

        var currentCompanyId = _tenantProvider.CompanyId;

        if (!currentCompanyId.HasValue)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));

        if (currentCompanyId.HasValue)
        {
            var nameExists = await _readRepository.ExistsActiveLocationWithName(request.Name, currentCompanyId.Value, ct);
            if (nameExists)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "A location with this name already exists in your company."));
            
        }

        if (!string.IsNullOrWhiteSpace(request.TimeZoneId))
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId.Trim());
            }
            catch (TimeZoneNotFoundException)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.TimeZoneId), "The provided timezone is invalid."));
            }
            catch (InvalidTimeZoneException)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.TimeZoneId), "The provided timezone is invalid."));
            }
        }


        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
