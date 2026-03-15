using OnTimeScheduling.Domain.Entities.Appointments;

namespace OnTimeScheduling.Application.Repositories.Appointments;

public interface IAppointmentWriteOnlyRepository
{
    Task Add(Appointment appointment, CancellationToken ct = default);
    void Update(Appointment appointment);
}
