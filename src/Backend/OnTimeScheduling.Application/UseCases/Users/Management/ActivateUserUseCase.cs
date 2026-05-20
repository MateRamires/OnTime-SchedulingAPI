using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class ActivateUserUseCase : IActivateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserUseCase(IUserRepository userRepository, ITenantProvider tenantProvider, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var user = await _userRepository.GetByIdAndCompanyIncludingInactive(userId, companyId, ct)
            ?? throw new NotFoundException("User not found.");

        user.Activate();

        _userRepository.Update(user);
        await _unitOfWork.Commit(ct);
    }

}
