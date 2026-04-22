using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Clients;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Clients;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class RegisterClientUseCase : IRegisterClientUseCase
{
    private readonly IClientWriteOnlyRepository _clientWriteRepository;
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterClientUseCase(
        IClientWriteOnlyRepository clientWriteRepository,
        IClientReadOnlyRepository clientReadRepository,
        IUnitOfWork unitOfWork)
    {
        _clientWriteRepository = clientWriteRepository;
        _clientReadRepository = clientReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterClientJson> ExecuteAsync(RequestRegisterClientJson request, CancellationToken ct = default)
    {
        request.Name = request.Name?.Trim() ?? string.Empty;
        request.Phone = request.Phone?.Trim() ?? string.Empty;
        request.Email = request.Email?.Trim();

        await Validate(request, ct);

        var client = new Client(request.Name, request.Phone, request.Email);

        await _clientWriteRepository.Add(client, ct);
        await _unitOfWork.Commit(ct);

        return new ResponseRegisterClientJson
        {
            Id = client.Id,
            Name = client.Name
        };
    }

    private async Task Validate(RequestRegisterClientJson request, CancellationToken ct)
    {
        var validator = new RegisterClientValidator();
        var result = validator.Validate(request);

        var phoneAlreadyExists = await _clientReadRepository.ExistsActiveByPhone(request.Phone, ct);
        if (phoneAlreadyExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "A client with this phone already exists."));

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }

}
