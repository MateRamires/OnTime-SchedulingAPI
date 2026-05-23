using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class UnlinkProfessionalServiceUseCase : IUnlinkProfessionalServiceUseCase
{
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadOnlyRepository;
    private readonly IProfessionalServiceWriteOnlyRepository _professionalServiceWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnlinkProfessionalServiceUseCase(IProfessionalServiceReadOnlyRepository professionalServiceReadOnlyRepository, IProfessionalServiceWriteOnlyRepository professionalServiceWriteOnlyRepository, IUnitOfWork unitOfWork)
    {
        _professionalServiceReadOnlyRepository = professionalServiceReadOnlyRepository;
        _professionalServiceWriteOnlyRepository = professionalServiceWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid serviceId, Guid professionalId, CancellationToken ct = default)
    {
        var linked = await _professionalServiceReadOnlyRepository.Exists(professionalId, serviceId, ct);
        if (!linked)
            throw new NotFoundException("Service-professional link not found.");

        await _professionalServiceWriteOnlyRepository.Delete(professionalId, serviceId, ct);
        await _unitOfWork.Commit(ct);
    }

}
