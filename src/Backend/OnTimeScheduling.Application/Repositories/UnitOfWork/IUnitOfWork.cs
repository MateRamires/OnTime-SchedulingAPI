namespace OnTimeScheduling.Application.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> Commit(CancellationToken ct = default);
}
