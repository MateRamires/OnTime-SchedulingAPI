using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class GetClientsUseCase : IGetClientsUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;

    public GetClientsUseCase(IClientReadOnlyRepository clientReadRepository)
    {
        _clientReadRepository = clientReadRepository;
    }

    public async Task<ResponsePagedResultJson<ResponseClientJson>> ExecuteAsync(RequestPaginationQuery pagination, CancellationToken ct = default)
    {
        var (clients, totalItems) = await _clientReadRepository.GetAllActiveAsync(pagination.Skip, pagination.Size, ct);

        var items = clients.Select(c => new ResponseClientJson
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email
        }).ToList();

        return new ResponsePagedResultJson<ResponseClientJson>
        {
            Page = pagination.Page,
            Size = pagination.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pagination.Size),
            Items = items
        };

    }

}
