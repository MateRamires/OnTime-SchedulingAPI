using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;
using CommunicationRecordStatus = OnTimeScheduling.Communication.Enums.RecordStatus;

namespace OnTimeScheduling.Application.UseCases.Clients;

public class GetClientByIdUseCase : IGetClientByIdUseCase
{
    private readonly IClientReadOnlyRepository _clientReadRepository;

    public GetClientByIdUseCase(IClientReadOnlyRepository clientReadRepository)
    {
        _clientReadRepository = clientReadRepository;
    }

    public async Task<ResponseClientJson> ExecuteAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await _clientReadRepository.GetByIdAsync(clientId, ct)
            ?? throw new NotFoundException("Client not found.");

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
