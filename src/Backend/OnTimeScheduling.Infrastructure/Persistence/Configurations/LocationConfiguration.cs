using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Locations;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId).IsRequired();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(255);
    }
}
