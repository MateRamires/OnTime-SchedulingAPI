using OnTimeScheduling.Domain.Entities.ScheduleBlocks;

namespace OnTimeScheduling.Application.Repositories.ScheduleBlocks;

public interface IScheduleBlockReadOnlyRepository
{
    Task<ScheduleBlock?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<ScheduleBlockDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default);

    Task<(List<ScheduleBlockDetails> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        Guid? professionalId,
        Guid? locationId,
        DateTime? startTimeUtc,
        DateTime? endTimeUtc,
        bool includeExpired,
        CancellationToken ct = default);

    Task<bool> HasOverlappingBlockForAppointmentAsync(
        Guid professionalId,
        Guid locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default,
        Guid? ignoredBlockId = null);

    Task<List<ScheduleBlock>> GetOverlappingBlocksForAppointmentAsync(
        Guid professionalId,
        Guid locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default);

}
