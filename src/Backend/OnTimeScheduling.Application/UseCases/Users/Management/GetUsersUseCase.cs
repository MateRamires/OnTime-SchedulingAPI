using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Users.Management.Mapper;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class GetUsersUseCase : IGetUsersUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetUsersUseCase(IUserRepository userRepository, ITenantProvider tenantProvider)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponsePagedResultJson<ResponseUserJson>> ExecuteAsync(RequestPaginationQuery pagination, UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        if (role == UserRole.SUPER_ADMIN)
            throw new ErrorOnValidationException(["SuperAdmin users are not managed through company user endpoints."]);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (users, totalItems) = await _userRepository.GetCompanyUsers(companyId, pagination.Skip, pagination.Size, role, status, searchTerm, ct);
        var items = users.Select(UserResponseMapper.Map).ToList();

        return new ResponsePagedResultJson<ResponseUserJson>
        {
            Page = pagination.Page,
            Size = pagination.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pagination.Size),
            Items = items
        };

    }

}
