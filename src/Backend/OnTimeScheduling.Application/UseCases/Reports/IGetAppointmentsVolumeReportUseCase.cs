using OnTimeScheduling.Communication.Requests.Reports;
using OnTimeScheduling.Communication.Responses.Reports;

namespace OnTimeScheduling.Application.UseCases.Reports;

public interface IGetAppointmentsVolumeReportUseCase
{
    Task<ResponseAppointmentsVolumeReportJson> ExecuteAsync(
        RequestAppointmentsVolumeReportJson request,
        CancellationToken ct = default);
}
