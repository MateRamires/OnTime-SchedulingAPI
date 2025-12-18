
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Entities.User;
using System.Threading;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.Touch();
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    public Task<int> CommitAsync(CancellationToken ct = default)
       => SaveChangesAsync(ct);
}
