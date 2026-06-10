using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.UseCases.ScheduleBlocks.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public class GetScheduleBlockByIdUseCase : IGetScheduleBlockByIdUseCase
{
    private readonly IScheduleBlockReadOnlyRepository _readRepository;

    public GetScheduleBlockByIdUseCase(IScheduleBlockReadOnlyRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ResponseScheduleBlockJson> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var block = await _readRepository.GetDetailsByIdAsync(id, ct)
            ?? throw new NotFoundException("Schedule block not found.");

        return ScheduleBlockResponseMapper.Map(block);
    }

}
