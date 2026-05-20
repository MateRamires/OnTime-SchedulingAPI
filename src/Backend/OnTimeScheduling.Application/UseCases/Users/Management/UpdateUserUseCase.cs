using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Exceptions.ExceptionBase;
using DomainUserRole = OnTimeScheduling.Domain.Enums.UserRole;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid userId, RequestUpdateUserJson request, CancellationToken ct = default)
    {
        request.Name = request.Name.FormatName();
        request.Email = request.Email.SanitizeEmail();

        await Validate(userId, request, ct);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var user = await _userRepository.GetByIdAndCompanyIncludingInactive(userId, companyId, ct)
            ?? throw new NotFoundException("User not found.");

        user.UpdateInternalProfile(request.Name, request.Email, (DomainUserRole)(int)request.Role);

        _userRepository.Update(user);
        await _unitOfWork.Commit(ct);
    }

    private async Task Validate(Guid userId, RequestUpdateUserJson request, CancellationToken ct)
    {
        var validator = new UpdateUserValidator();
        var result = validator.Validate(request);

        if (!_tenantProvider.CompanyId.HasValue)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The authenticated user does not have a valid tenant context."));

        var loggedUser = _loggedUser.GetUser();
        if (loggedUser.Id == userId && loggedUser.Role != (DomainUserRole)(int)request.Role)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.Role), "Users cannot change their own role."));

        var emailExists = await _userRepository.EmailExistsExceptId(request.Email, userId, ct);
        if (emailExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.Email), "The Email is Already Registered!"));

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }

}
