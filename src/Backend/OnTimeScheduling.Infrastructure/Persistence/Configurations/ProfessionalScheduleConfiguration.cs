using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class ProfessionalScheduleConfiguration : IEntityTypeConfiguration<ProfessionalSchedule>
{
    public void Configure(EntityTypeBuilder<ProfessionalSchedule> builder)
    {
        builder.ToTable("professional_schedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(x => x.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("time without time zone") // Padrão seguro para PostgreSQL
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at_utc").IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Professional)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CompanyId, x.UserId, x.DayOfWeek });        
        builder.HasIndex(x => new { x.CompanyId, x.LocationId, x.DayOfWeek });
    }
}
