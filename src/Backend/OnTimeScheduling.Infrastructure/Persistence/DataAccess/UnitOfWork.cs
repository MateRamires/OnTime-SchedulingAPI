using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Commit(CancellationToken ct = default)
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.Touch();
        }

        try
        {
            return await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsAppointmentOverlapConstraintViolation(ex))
        {
            throw new ConflictException("The selected time slot is no longer available due to an overlapping appointment.");
        }
    }

    private static bool IsAppointmentOverlapConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ExclusionViolation,
            ConstraintName: "ex_appointments_no_professional_overlap"
        };
    }
}
