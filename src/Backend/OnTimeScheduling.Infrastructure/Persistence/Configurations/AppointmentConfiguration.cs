using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", tb =>
        {
            tb.HasCheckConstraint(
                "ck_appointments_start_before_end",
                "start_time < end_time"
            );
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.CompanyId).HasColumnName("company_id").IsRequired();
        builder.Property(x => x.ProfessionalId).HasColumnName("professional_id").IsRequired();
        builder.Property(x => x.ServiceId).HasColumnName("service_id").IsRequired();
        builder.Property(x => x.LocationId).HasColumnName("location_id").IsRequired();

        builder.Property(x => x.ClientName).HasColumnName("client_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.ClientPhone).HasColumnName("client_phone").HasMaxLength(20).IsRequired();

        builder.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("timestamp with time zone").IsRequired();

        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();

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

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // By professional + time range (main schedule query)
        builder.HasIndex(x => new { x.CompanyId, x.ProfessionalId, x.StartTime })
            .HasDatabaseName("ix_appointments_company_professional_start");

        // By location + time range (location/day view)
        builder.HasIndex(x => new { x.CompanyId, x.LocationId, x.StartTime })
            .HasDatabaseName("ix_appointments_company_location_start");

        // Company-wide timeline (day/week view)
        builder.HasIndex(x => new { x.CompanyId, x.StartTime })
            .HasDatabaseName("ix_appointments_company_start");
    }
}
