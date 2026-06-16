using OnTimeScheduling.Communication.Requests.Reports;
using OnTimeScheduling.Communication.Responses.Reports;

namespace OnTimeScheduling.Application.UseCases.Reports;

public interface IGetProfessionalOccupancyReportUseCase
{
    Task<ResponseProfessionalOccupancyReportJson> ExecuteAsync(
        RequestProfessionalOccupancyReportJson request,
        CancellationToken ct = default);
}
