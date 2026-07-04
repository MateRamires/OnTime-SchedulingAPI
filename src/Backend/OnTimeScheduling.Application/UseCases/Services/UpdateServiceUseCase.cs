using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Application.Validators.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class UpdateServiceUseCase : IUpdateServiceUseCase
{
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;
    private readonly IServiceWriteOnlyRepository _serviceWriteOnlyRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAgendaConcurrencyGuard _agendaConcurrencyGuard;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceUseCase(
        IServiceReadOnlyRepository serviceReadOnlyRepository,
        IServiceWriteOnlyRepository serviceWriteOnlyRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IAgendaConcurrencyGuard agendaConcurrencyGuard,
        IUnitOfWork unitOfWork)
    {
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
        _serviceWriteOnlyRepository = serviceWriteOnlyRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _agendaConcurrencyGuard = agendaConcurrencyGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid serviceId, RequestUpdateServiceJson request, CancellationToken ct = default)
    {
        request.Name = request.Name?.Trim() ?? string.Empty;
        request.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        await _agendaConcurrencyGuard.ExecuteAsync(
            [AgendaConcurrencyLockKey.ForService(serviceId)],
            async lockedCt =>
            {
                await Validate(serviceId, request, lockedCt);

                var service = await _serviceReadOnlyRepository.GetByIdAsync(serviceId, lockedCt)
                    ?? throw new NotFoundException("Service not found.");

                if (service.DurationInMinutes != request.DurationInMinutes)
                {
                    var hasFutureAppointments = await _appointmentReadRepository
                        .HasFutureScheduledAppointmentsAsync(serviceId: serviceId, ct: lockedCt);

                    if (hasFutureAppointments)
                        throw new ConflictException("Cannot change the duration of a service with future scheduled appointments. Cancel or reschedule those appointments first.");
                }

                service.Update(request.Name, request.Description, request.Price, request.DurationInMinutes);

                _serviceWriteOnlyRepository.Update(service);
                await _unitOfWork.Commit(lockedCt);
            },
            ct);
    }

    private async Task Validate(Guid serviceId, RequestUpdateServiceJson request, CancellationToken ct)
    {
        var validator = new UpdateServiceValidator();
        var result = validator.Validate(request);

        var nameExists = await _serviceReadOnlyRepository.ExistsWithNameExceptId(request.Name, serviceId, ct);
        if (nameExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.Name), "A service with this name is already registered in your company."));

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }
}
