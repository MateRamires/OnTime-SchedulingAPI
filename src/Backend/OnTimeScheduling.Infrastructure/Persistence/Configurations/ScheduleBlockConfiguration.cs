using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Entities.ScheduleBlocks;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class ScheduleBlockConfiguration : IEntityTypeConfiguration<ScheduleBlock>
{
    public void Configure(EntityTypeBuilder<ScheduleBlock> builder)
    {
        builder.ToTable("schedule_blocks", tb =>
        {
            tb.HasCheckConstraint(
                "ck_schedule_blocks_start_before_end",
                "start_time < end_time");

            tb.HasCheckConstraint(
                "ck_schedule_blocks_has_scope",
                "professional_id IS NOT NULL OR location_id IS NOT NULL");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CompanyId).HasColumnName("company_id").IsRequired();
        builder.Property(x => x.ProfessionalId).HasColumnName("professional_id");
        builder.Property(x => x.LocationId).HasColumnName("location_id");
        builder.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at_utc").IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Professional)
            .WithMany()
            .HasForeignKey(x => x.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CompanyId, x.ProfessionalId, x.StartTime })
            .HasDatabaseName("ix_schedule_blocks_company_professional_start");

        builder.HasIndex(x => new { x.CompanyId, x.LocationId, x.StartTime })
            .HasDatabaseName("ix_schedule_blocks_company_location_start");

        builder.HasIndex(x => new { x.CompanyId, x.StartTime, x.EndTime })
            .HasDatabaseName("ix_schedule_blocks_company_time_range");
    }

}
