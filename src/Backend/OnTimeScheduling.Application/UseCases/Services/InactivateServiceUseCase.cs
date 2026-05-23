using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class InactivateServiceUseCase : IInactivateServiceUseCase
{
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;
    private readonly IServiceWriteOnlyRepository _serviceWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateServiceUseCase(IServiceReadOnlyRepository serviceReadOnlyRepository, IServiceWriteOnlyRepository serviceWriteOnlyRepository, IUnitOfWork unitOfWork)
    {
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
        _serviceWriteOnlyRepository = serviceWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid serviceId, CancellationToken ct = default)
    {
        var service = await _serviceReadOnlyRepository.GetByIdAsync(serviceId, ct)
            ?? throw new NotFoundException("Service not found.");

        service.Inactivate();
        _serviceWriteOnlyRepository.Update(service);
        await _unitOfWork.Commit(ct);
    }

}
