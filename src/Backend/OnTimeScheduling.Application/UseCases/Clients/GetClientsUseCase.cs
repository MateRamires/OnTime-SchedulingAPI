using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class GetClientsUseCase : IGetClientsUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;

    public GetClientsUseCase(IClientReadOnlyRepository clientReadRepository)
    {
        _clientReadRepository = clientReadRepository;
    }

    public async Task<List<ResponseClientJson>> ExecuteAsync(CancellationToken ct = default)
    {
        var clients = await _clientReadRepository.GetAllActiveAsync(ct);

        return clients.Select(c => new ResponseClientJson
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email
        }).ToList();
    }

}
