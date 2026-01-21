using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Login;

public class LoginUseCase : ILoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    public LoginUseCase(IUserRepository userRepository, IPasswordHashService passwordHashService, IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<ResponseLoginJson> ExecuteAsync(RequestLoginJson request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmail(request.Email, ct);

        if (user is null)
        {
            throw new InvalidLoginException("Credenciais inválidas!");
        }

        var passwordResult = _passwordHashService.Verify(user.PasswordHash, request.Password);

        if (passwordResult == PasswordVerifyResult.Failed)
        {
            throw new InvalidLoginException("Credenciais inválidas!");
        }

        var accessToken = _accessTokenGenerator.Generate(user);

        return new ResponseLoginJson
        {
            Name = user.Name,
            AccessToken = accessToken
        };
    }
}
