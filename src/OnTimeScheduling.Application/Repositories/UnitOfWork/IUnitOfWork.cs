namespace OnTimeScheduling.Application.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
