using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class GetUserByIdUseCase : IGetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetUserByIdUseCase(IUserRepository userRepository, ITenantProvider tenantProvider)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseUserJson> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var user = await _userRepository.GetByIdAndCompanyIncludingInactive(userId, companyId, ct)
            ?? throw new NotFoundException("User not found.");

        return UserResponseMapper.Map(user);
    }

}
