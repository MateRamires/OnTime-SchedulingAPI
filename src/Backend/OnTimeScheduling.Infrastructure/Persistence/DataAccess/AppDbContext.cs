
using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Company> Companies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
