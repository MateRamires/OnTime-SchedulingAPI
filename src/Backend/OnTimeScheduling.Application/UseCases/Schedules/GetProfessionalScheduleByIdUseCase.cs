using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.UseCases.Schedules.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class GetProfessionalScheduleByIdUseCase : IGetProfessionalScheduleByIdUseCase
{
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;

    public GetProfessionalScheduleByIdUseCase(IProfessionalScheduleReadOnlyRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ResponseProfessionalScheduleJson> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var schedule = await _readRepository.GetDetailsByIdAsync(id, ct)
            ?? throw new NotFoundException("Professional schedule not found.");

        return ProfessionalScheduleResponseMapper.Map(schedule);
    }
}
