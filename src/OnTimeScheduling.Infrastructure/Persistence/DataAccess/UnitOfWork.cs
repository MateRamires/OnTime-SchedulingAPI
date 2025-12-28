using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Domain.Entities.DefaultEntity;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> Commit(CancellationToken ct = default)
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.Touch();
        }

        return _dbContext.SaveChangesAsync(ct);
    }
}
