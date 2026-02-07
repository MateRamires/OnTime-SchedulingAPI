using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class RegisterCompanyUseCase : IRegisterCompanyUseCase
{
    private readonly ICompanyWriteOnlyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    public RegisterCompanyUseCase(ICompanyWriteOnlyRepository companyWriteOnlyRepository, IUserRepository userRepository, 
        IUnitOfWork unitOfWork, IPasswordHashService passwordHashService)
    {
        _companyRepository = companyWriteOnlyRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
    }
    public async Task<ResponseRegisterCompanyJson> ExecuteAsync(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
        await Validate(request, ct);
    }

    private async Task Validate(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
        var emailExists = await _userRepository.EmailExists(request.Email, ct);

        if (emailExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The Email is Already Registered!"));


    }
}
