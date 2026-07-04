using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class ActivateClientUseCase : IActivateClientUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IClientWriteOnlyRepository _clientWriteRepository;
    private readonly IAgendaConcurrencyGuard _agendaConcurrencyGuard;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateClientUseCase(
        IClientReadOnlyRepository clientReadRepository,
        IClientWriteOnlyRepository clientWriteRepository,
        IAgendaConcurrencyGuard agendaConcurrencyGuard,
        IUnitOfWork unitOfWork)
    {
        _clientReadRepository = clientReadRepository;
        _clientWriteRepository = clientWriteRepository;
        _agendaConcurrencyGuard = agendaConcurrencyGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid clientId, CancellationToken ct = default)
    {
        await _agendaConcurrencyGuard.ExecuteAsync(
            [AgendaConcurrencyLockKey.ForClient(clientId)],
            async lockedCt =>
            {
                var client = await _clientReadRepository.GetByIdAsync(clientId, lockedCt)
                    ?? throw new NotFoundException("Client not found.");

                client.Activate();
                _clientWriteRepository.Update(client);

                await _unitOfWork.Commit(lockedCt);
            },
            ct);
    }
}
