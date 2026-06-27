using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Clients;
using OnTimeScheduling.Domain.Enums;
using CommunicationRecordStatus = OnTimeScheduling.Communication.Enums.RecordStatus;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class GetClientsUseCase : IGetClientsUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;

    public GetClientsUseCase(IClientReadOnlyRepository clientReadRepository)
    {
        _clientReadRepository = clientReadRepository;
    }

    public async Task<ResponsePagedResultJson<ResponseClientJson>> ExecuteAsync(
        RequestPaginationQuery pagination,
        RecordStatus? status = null,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var (clients, totalItems) = await _clientReadRepository.GetAllAsync(
            pagination.Skip,
            pagination.Size,
            status,
            searchTerm,
            ct);

        var items = clients.Select(Map).ToList();

        return new ResponsePagedResultJson<ResponseClientJson>
        {
            Page = pagination.Page,
            Size = pagination.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pagination.Size),
            Items = items
        };
    }

    private static ResponseClientJson Map(Client client)
    {
        return new ResponseClientJson
        {
            Id = client.Id,
            Name = client.Name,
            Phone = client.Phone,
            Email = client.Email,
            Status = (CommunicationRecordStatus)(int)client.Status,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}
