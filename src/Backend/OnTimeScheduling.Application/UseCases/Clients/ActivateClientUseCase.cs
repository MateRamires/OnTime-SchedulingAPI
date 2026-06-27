using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class ActivateClientUseCase : IActivateClientUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IClientWriteOnlyRepository _clientWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateClientUseCase(
        IClientReadOnlyRepository clientReadRepository,
        IClientWriteOnlyRepository clientWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _clientReadRepository = clientReadRepository;
        _clientWriteRepository = clientWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await _clientReadRepository.GetByIdAsync(clientId, ct)
            ?? throw new NotFoundException("Client not found.");

        client.Activate();
        _clientWriteRepository.Update(client);

        await _unitOfWork.Commit(ct);
    }
}
