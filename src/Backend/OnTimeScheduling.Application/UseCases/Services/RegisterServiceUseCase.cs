using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Services;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class RegisterServiceUseCase : IRegisterServiceUseCase
{
    private readonly IServiceWriteOnlyRepository _serviceWriteRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterServiceUseCase(
        IServiceWriteOnlyRepository serviceWriteRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceWriteRepository = serviceWriteRepository;
        _serviceReadRepository = serviceReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterServiceJson> ExecuteAsync(RequestRegisterServiceJson request, CancellationToken ct = default)
    {
        //TODO: request can have a professionalIds so the user can register the service + a bunch of users that can do said service
        request.Name = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.Description))
            request.Description = request.Description.Trim();

        //TODO: Make the Validator safer for numbers (price + duration). Add a new error if the user sends a invalid caracter (letter or special caracter)
        await Validate(request, ct);

        var service = new Service(
            name: request.Name,
            description: request.Description,
            price: request.Price,
            durationInMinutes: request.DurationInMinutes
        );

        await _serviceWriteRepository.Add(service, ct);
        await _unitOfWork.Commit();

        return new ResponseRegisterServiceJson
        {
            Id = service.Id, 
            Name = service.Name
        };
    }

    private async Task Validate(RequestRegisterServiceJson request, CancellationToken ct = default)
    {
        var validator = new RegisterServiceValidator();
        var result = validator.Validate(request);

        var exists = await _serviceReadRepository.ExistsActiveWithName(request.Name, ct);
        if (exists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "A service with this name is already registered in your company."));

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
