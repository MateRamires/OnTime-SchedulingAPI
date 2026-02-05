using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SocialReason)
            .HasColumnName("social_reason")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FantasyName)
            .HasColumnName("fantasy_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Document)
            .HasColumnName("document")
            .HasMaxLength(20) 
            .IsRequired();

        builder.HasIndex(x => x.Document).IsUnique();

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(200)
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
    }
}
