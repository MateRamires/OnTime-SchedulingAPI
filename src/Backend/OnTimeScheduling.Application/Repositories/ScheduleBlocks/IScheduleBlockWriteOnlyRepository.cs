namespace OnTimeScheduling.Application.Repositories.ScheduleBlocks;

public interface IScheduleBlockWriteOnlyRepository
{
    Task AddAsync(ScheduleBlock block, CancellationToken ct = default);
    void Update(ScheduleBlock block);
    void Delete(ScheduleBlock block);

}
