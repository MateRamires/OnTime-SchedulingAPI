using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.ScheduleBlocks.Mapper;
using OnTimeScheduling.Application.Validators.ScheduleBlocks;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public class GetScheduleBlocksUseCase : IGetScheduleBlocksUseCase
{
    private readonly IScheduleBlockReadOnlyRepository _readRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetScheduleBlocksUseCase(IScheduleBlockReadOnlyRepository readRepository, ITenantProvider tenantProvider)
    {
        _readRepository = readRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponsePagedResultJson<ResponseScheduleBlockJson>> ExecuteAsync(RequestGetScheduleBlocksJson request, CancellationToken ct = default)
    {
        Validate(request);

        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (blocks, totalItems) = await _readRepository.GetAllAsync(
            request.Skip,
            request.Size,
            request.ProfessionalId,
            request.LocationId,
            request.StartTime,
            request.EndTime,
            request.IncludeExpired,
            ct);

        var items = blocks.Select(ScheduleBlockResponseMapper.Map).ToList();

        return new ResponsePagedResultJson<ResponseScheduleBlockJson>
        {
            Page = request.Page,
            Size = request.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.Size),
            Items = items
        };
    }

    private static void Validate(RequestGetScheduleBlocksJson request)
    {
        var validator = new GetScheduleBlocksValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }

}
