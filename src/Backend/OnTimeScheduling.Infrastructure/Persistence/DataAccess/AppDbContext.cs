
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
