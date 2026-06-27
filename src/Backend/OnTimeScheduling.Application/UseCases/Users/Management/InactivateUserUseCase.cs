using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class InactivateUserUseCase : IInactivateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateUserUseCase(
        IUserRepository userRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        ITenantProvider tenantProvider,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _tenantProvider = tenantProvider;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var loggedUser = _loggedUser.GetUser();
        if (loggedUser.Id == userId)
            throw new ErrorOnValidationException(["Users cannot inactivate their own account."]);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var user = await _userRepository.GetByIdAndCompanyIncludingInactive(userId, companyId, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.Role == UserRole.PROVIDER)
        {
            var hasFutureAppointments = await _appointmentReadRepository
                .HasFutureScheduledAppointmentsAsync(professionalId: userId, ct: ct);

            if (hasFutureAppointments)
                throw new ConflictException("Cannot inactivate a provider with future scheduled appointments. Cancel or reschedule those appointments first.");
        }

        user.Inactivate();

        _userRepository.Update(user);
        await _unitOfWork.Commit(ct);
    }
}
