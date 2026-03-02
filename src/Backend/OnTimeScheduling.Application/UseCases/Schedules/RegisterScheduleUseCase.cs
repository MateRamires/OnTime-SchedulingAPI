using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Schedules;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Schedules;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class RegisterScheduleUseCase
{
    private readonly IProfessionalScheduleWriteOnlyRepository _writeRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterScheduleUseCase(
        IProfessionalScheduleWriteOnlyRepository writeRepository,
        IProfessionalScheduleReadOnlyRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterScheduleJson> ExecuteAsync(RequestRegisterScheduleJson request, CancellationToken ct = default)
    {
        await Validate(request, ct);

        var schedule = new ProfessionalSchedule(
            userId: request.UserId,
            locationId: request.LocationId,
            dayOfWeek: request.DayOfWeek,
            startTime: request.StartTime,
            endTime: request.EndTime
        );

        await _writeRepository.Add(schedule, ct);
        await _unitOfWork.Commit();

        return new ResponseRegisterScheduleJson
        {
            Id = schedule.Id
        };
    }

    private async Task Validate(RequestRegisterScheduleJson request, CancellationToken ct = default)
    {
        var validator = new RegisterScheduleValidator();
        var result = validator.Validate(request);

        var hasOverlap = await _readRepository.HasOverlappingSchedule(
            request.UserId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            ct);

        if (hasOverlap)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                string.Empty,
                "This schedule block overlaps with an existing schedule for this professional on the selected day."));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
