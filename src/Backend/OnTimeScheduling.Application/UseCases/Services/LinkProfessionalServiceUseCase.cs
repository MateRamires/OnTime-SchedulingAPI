using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Services;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class LinkProfessionalServiceUseCase : ILinkProfessionalServiceUseCase
{
    private readonly IProfessionalServiceWriteOnlyRepository _writeRepository;
    private readonly IProfessionalServiceReadOnlyRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkProfessionalServiceUseCase(
        IProfessionalServiceWriteOnlyRepository writeRepository,
        IProfessionalServiceReadOnlyRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
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

        var alreadyLinked = await _readRepository.Exists(request.UserId, request.ServiceId, ct);
        if (alreadyLinked)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "This service is already linked to this professional."));

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
