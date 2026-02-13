
using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Entities.Locations;
using OnTimeScheduling.Domain.Entities.User;
using System.Reflection;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class AppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies { get; set; }
    public DbSet<Location> Locations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void ConfigureTenantFilter<T>(ModelBuilder builder) where T : class, IMustHaveTenant
    {
        builder.Entity<T>().HasQueryFilter(e => e.CompanyId == _tenantProvider.CompanyId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentCompanyId = _tenantProvider.CompanyId;

        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                if (currentCompanyId.HasValue)
                {
                    entry.Entity.CompanyId = currentCompanyId.Value;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
