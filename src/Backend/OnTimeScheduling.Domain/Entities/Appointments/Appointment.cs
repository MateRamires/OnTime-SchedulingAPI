using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Appointments;

public class Appointment : TenantEntity
{
    public Guid ProfessionalId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid LocationId { get; private set; }

    // TODO: Create a client entity.
    public string ClientName { get; private set; } = string.Empty;
    public string ClientPhone { get; private set; } = string.Empty;

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public User.User Professional { get; private set; } = null!;
    public Services.Service Service { get; private set; } = null!;
    public Locations.Location Location { get; private set; } = null!;

    private Appointment() { } 

    public Appointment(
        Guid professionalId,
        Guid serviceId,
        Guid locationId,
        string clientName,
        string clientPhone,
        DateTime startTime,
        DateTime endTime)
    {
        ProfessionalId = professionalId;
        ServiceId = serviceId;
        LocationId = locationId;
        ClientName = clientName;
        ClientPhone = clientPhone;

        SetTimes(startTime, endTime);
        Status = AppointmentStatus.Scheduled; 
    }

    private void SetTimes(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new DomainRuleException("The start time must be before the end time.");

        if (start < DateTime.UtcNow)
            throw new DomainRuleException("Appointments cannot be scheduled in the past.");

        StartTime = start.ToUniversalTime();
        EndTime = end.ToUniversalTime();
    }

    public void Reschedule(
       Guid professionalId,
       Guid serviceId,
       Guid locationId,
       string clientName,
       string clientPhone,
       DateTime startTime,
       DateTime endTime)
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainRuleException("Only scheduled appointments can be edited.");

        ProfessionalId = professionalId;
        ServiceId = serviceId;
        LocationId = locationId;
        ClientName = clientName;
        ClientPhone = clientPhone;

        SetTimes(startTime, endTime);
    }


    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.NoShow)
            throw new DomainRuleException("Cannot cancel an appointment that was already finalized.");

        Status = AppointmentStatus.Canceled;
    }

    public void MarkAsCompleted()
    {
        EnsureCanSetProviderOutcome();
        Status = AppointmentStatus.Completed;
    }

    public void MarkAsNoShow()
    {
        EnsureCanSetProviderOutcome();
        Status = AppointmentStatus.NoShow;
    }

    private void EnsureCanSetProviderOutcome()
    {
        if (Status == AppointmentStatus.Canceled)
            throw new DomainRuleException("Cannot set outcome for a canceled appointment.");

        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.NoShow)
            throw new DomainRuleException("Appointment outcome has already been finalized.");
    }

}
