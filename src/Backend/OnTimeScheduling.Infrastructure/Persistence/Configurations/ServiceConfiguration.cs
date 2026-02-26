using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(10,2)") 
            .IsRequired();

        builder.Property(x => x.DurationInMinutes)
            .HasColumnName("duration_in_minutes")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}
