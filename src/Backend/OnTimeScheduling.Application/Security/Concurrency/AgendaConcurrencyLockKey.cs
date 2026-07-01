namespace OnTimeScheduling.Application.Security.Concurrency;

public sealed record AgendaConcurrencyLockKey(string ResourceType, Guid ResourceId)
{
    public static AgendaConcurrencyLockKey ForAppointment(Guid appointmentId) =>
        new("appointment", appointmentId);

    public static AgendaConcurrencyLockKey ForClient(Guid clientId) =>
        new("client", clientId);

    public static AgendaConcurrencyLockKey ForLocation(Guid locationId) =>
        new("location", locationId);

    public static AgendaConcurrencyLockKey ForProfessional(Guid professionalId) =>
        new("professional", professionalId);

    public static AgendaConcurrencyLockKey ForProfessionalSchedule(Guid scheduleId) =>
        new("professional-schedule", scheduleId);

    public static AgendaConcurrencyLockKey ForService(Guid serviceId) =>
        new("service", serviceId);
}
