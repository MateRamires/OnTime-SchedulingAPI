using OnTimeScheduling.Application.Repositories.Appointments;
using CommunicationAppointmentStatus = OnTimeScheduling.Communication.Enums.AppointmentStatus;
using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments.Mapper;

public class AppointmentResponseMapper
{
    public static ResponseAppointmentSummaryJson MapSummary(AppointmentDetails appointment)
    {
        return new ResponseAppointmentSummaryJson
        {
            Id = appointment.AppointmentId,
            ClientId = appointment.ClientId,
            ClientName = appointment.ClientName,
            ClientPhone = appointment.ClientPhone,
            ClientEmail = appointment.ClientEmail,
            ProfessionalId = appointment.ProfessionalId,
            ProfessionalName = appointment.ProfessionalName,
            LocationId = appointment.LocationId,
            LocationName = appointment.LocationName,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.ServiceName,
            ServiceDurationInMinutes = appointment.ServiceDurationInMinutes,
            Status = (CommunicationAppointmentStatus)(int)appointment.Status,
            StartTimeUtc = appointment.StartTimeUtc,
            EndTimeUtc = appointment.EndTimeUtc
        };
    }

    public static ResponseAppointmentJson MapDetails(AppointmentDetails appointment)
    {
        return new ResponseAppointmentJson
        {
            Id = appointment.AppointmentId,
            Client = new ResponseAppointmentClientJson
            {
                Id = appointment.ClientId,
                Name = appointment.ClientName,
                Phone = appointment.ClientPhone,
                Email = appointment.ClientEmail
            },
            Professional = new ResponseAppointmentParticipantJson
            {
                Id = appointment.ProfessionalId,
                Name = appointment.ProfessionalName
            },
            Location = new ResponseAppointmentParticipantJson
            {
                Id = appointment.LocationId,
                Name = appointment.LocationName
            },
            Service = new ResponseAppointmentServiceJson
            {
                Id = appointment.ServiceId,
                Name = appointment.ServiceName,
                DurationInMinutes = appointment.ServiceDurationInMinutes,
                Price = appointment.ServicePrice
            },
            Status = (CommunicationAppointmentStatus)(int)appointment.Status,
            StartTimeUtc = appointment.StartTimeUtc,
            EndTimeUtc = appointment.EndTimeUtc,
            CreatedAtUtc = appointment.CreatedAtUtc,
            UpdatedAtUtc = appointment.UpdatedAtUtc
        };
    }

}
