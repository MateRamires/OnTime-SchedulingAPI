using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Clients;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class UpdateClientUseCase : IUpdateClientUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IClientWriteOnlyRepository _clientWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientUseCase(
        IClientReadOnlyRepository clientReadRepository,
        IClientWriteOnlyRepository clientWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _clientReadRepository = clientReadRepository;
        _clientWriteRepository = clientWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid clientId, RequestUpdateClientJson request, CancellationToken ct = default)
    {
        request.Name = request.Name?.Trim() ?? string.Empty;
        request.Phone = request.Phone?.Trim() ?? string.Empty;
        request.Email = request.Email?.Trim();

        var validator = new UpdateClientValidator();
        var validation = validator.Validate(request);

        if (!validation.IsValid)
            throw new ErrorOnValidationException(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var client = await _clientReadRepository.GetByIdAsync(clientId, ct)
            ?? throw new NotFoundException("Client not found.");

        client.Update(request.Name, request.Phone, request.Email);

        _clientWriteRepository.Update(client);
        await _unitOfWork.Commit(ct);
    }

}
