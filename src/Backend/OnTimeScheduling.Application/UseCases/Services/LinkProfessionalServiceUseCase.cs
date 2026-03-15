using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Services;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class LinkProfessionalServiceUseCase : ILinkProfessionalServiceUseCase
{
    private readonly IProfessionalServiceWriteOnlyRepository _writeRepository;
    private readonly IProfessionalServiceReadOnlyRepository _readRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;
    private readonly ITenantProvider _tenantProvider;

    private readonly IUnitOfWork _unitOfWork;

    public LinkProfessionalServiceUseCase(
        IProfessionalServiceWriteOnlyRepository writeRepository,
        IProfessionalServiceReadOnlyRepository readRepository,
        IUserRepository userRepository,
        IServiceReadOnlyRepository serviceReadOnlyRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _userRepository = userRepository;
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseLinkProfessionalServiceJson> ExecuteAsync(RequestLinkProfessionalServiceJson request, CancellationToken ct = default)
    {
        await Validate(request, ct);

        var professionalService = new ProfessionalService(
            userId: request.UserId,
            serviceId: request.ServiceId
        );

        await _writeRepository.Add(professionalService, ct);
        await _unitOfWork.Commit();

        return new ResponseLinkProfessionalServiceJson
        {
            UserId = professionalService.UserId,
            ServiceId = professionalService.ServiceId,
            Message = "Service successfully linked to the professional."
        };
    }

    private async Task Validate(RequestLinkProfessionalServiceJson request, CancellationToken ct = default)
    {
        var validator = new LinkProfessionalServiceValidator();
        var result = validator.Validate(request);

        if (!_tenantProvider.CompanyId.HasValue)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));

        var alreadyLinked = await _readRepository.Exists(request.UserId, request.ServiceId, ct);
        if (alreadyLinked)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "This service is already linked to this professional."));

        if (_tenantProvider.CompanyId.HasValue)
        {
            var companyId = _tenantProvider.CompanyId.Value;

            var professional = await _userRepository.GetByIdAndCompany(request.UserId, companyId, ct);
            if (professional is null)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Professional not found in this tenant."));
            else if (professional.Role != UserRole.PROVIDER)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Only provider users can be linked to a service."));
            

            var serviceExists = await _serviceReadOnlyRepository.ExistsActiveById(request.ServiceId, ct);
            if (!serviceExists)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Service not found in this tenant."));
        }


        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
