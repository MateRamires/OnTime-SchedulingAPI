using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Locations;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Locations;
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
        await Validate(request, ct);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var location = new Location(
            companyId: companyId,
            name: request.Name,
            address: request.Address
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

        if (currentCompanyId.HasValue)
        {
            var nameExists = await _readRepository.ExistsActiveLocationWithName(request.Name, currentCompanyId.Value, ct);
            if (nameExists)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "A location with this name already exists in your company."));
            
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
