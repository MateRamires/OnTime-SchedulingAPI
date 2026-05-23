using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class GetServiceByIdUseCase
{
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;

    public GetServiceByIdUseCase(IServiceReadOnlyRepository serviceReadOnlyRepository)
    {
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
    }

    public async Task<ResponseServiceJson> ExecuteAsync(Guid serviceId, CancellationToken ct = default)
    {
        var service = await _serviceReadOnlyRepository.GetByIdAsync(serviceId, ct)
            ?? throw new NotFoundException("Service not found.");

        return ServiceResponseMapper.Map(service);
    }

}
