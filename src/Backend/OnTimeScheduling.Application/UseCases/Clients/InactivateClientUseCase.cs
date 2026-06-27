using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class InactivateClientUseCase : IInactivateClientUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;
    private readonly IClientWriteOnlyRepository _clientWriteRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateClientUseCase(
        IClientReadOnlyRepository clientReadRepository,
        IClientWriteOnlyRepository clientWriteRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IUnitOfWork unitOfWork)
    {
        _clientReadRepository = clientReadRepository;
        _clientWriteRepository = clientWriteRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await _clientReadRepository.GetByIdAsync(clientId, ct)
            ?? throw new NotFoundException("Client not found.");

        var hasFutureAppointments = await _appointmentReadRepository
            .HasFutureScheduledAppointmentsAsync(clientId: clientId, ct: ct);

        if (hasFutureAppointments)
            throw new ConflictException("Cannot inactivate a client with future scheduled appointments. Cancel or reschedule those appointments first.");

        client.Inactivate();
        _clientWriteRepository.Update(client);

        await _unitOfWork.Commit(ct);
    }
}
