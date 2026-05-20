using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Users.Management.Mapper;
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

    public async Task<List<ResponseUserJson>> ExecuteAsync(UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        if (role == UserRole.SUPER_ADMIN)
            throw new ErrorOnValidationException(["SuperAdmin users are not managed through company user endpoints."]);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var users = await _userRepository.GetCompanyUsers(companyId, role, status, searchTerm, ct);

        return users.Select(UserResponseMapper.Map).ToList();
    }

}
