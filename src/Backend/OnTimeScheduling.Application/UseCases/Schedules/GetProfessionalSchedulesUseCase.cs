using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Schedules.Mapper;
using OnTimeScheduling.Application.Validators.Schedules;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class GetProfessionalSchedulesUseCase : IGetProfessionalSchedulesUseCase
{
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetProfessionalSchedulesUseCase(
        IProfessionalScheduleReadOnlyRepository readRepository,
        ITenantProvider tenantProvider)
    {
        _readRepository = readRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponsePagedResultJson<ResponseProfessionalScheduleJson>> ExecuteAsync(
        RequestGetProfessionalSchedulesJson request,
        CancellationToken ct = default)
    {
        Validate(request);

        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (schedules, totalItems) = await _readRepository.GetAllAsync(
            request.Skip,
            request.Size,
            request.ProfessionalId,
            request.LocationId,
            request.DayOfWeek,
            ct);

        var items = schedules.Select(ProfessionalScheduleResponseMapper.Map).ToList();

        return new ResponsePagedResultJson<ResponseProfessionalScheduleJson>
        {
            Page = request.Page,
            Size = request.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.Size),
            Items = items
        };
    }

    private static void Validate(RequestGetProfessionalSchedulesJson request)
    {
        var validator = new GetProfessionalSchedulesValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
