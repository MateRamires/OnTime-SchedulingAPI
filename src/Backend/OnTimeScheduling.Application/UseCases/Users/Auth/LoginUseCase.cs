using OnTimeScheduling.Application.Repositories.Auth;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenSettings _refreshTokenSettings;

    public LoginUseCase(IUserRepository userRepository, IPasswordHashService passwordHashService, IAccessTokenGenerator accessTokenGenerator, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, IRefreshTokenGenerator refreshTokenGenerator, IRefreshTokenSettings refreshTokenSettings)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _accessTokenGenerator = accessTokenGenerator;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenSettings = refreshTokenSettings;
    }

    public async Task<ResponseLoginJson> ExecuteAsync(RequestLoginJson request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidLoginException("Invalid credentials.");

        var user = await _userRepository.GetByEmail(request.Email, ct);

        if (user is null)
        {
            throw new InvalidLoginException("Invalid credentials.");
        }

        var passwordResult = _passwordHashService.Verify(user.PasswordHash, request.Password);

        if (passwordResult == PasswordVerifyResult.Failed)
        {
            throw new InvalidLoginException("Invalid credentials.");
        }

        if (passwordResult == PasswordVerifyResult.SuccessRehashNeeded)
        {
            var newHash = _passwordHashService.Hash(request.Password);

            user.UpdatePasswordHash(newHash);

            _userRepository.Update(user);
            await _unitOfWork.Commit(ct);
        }

        var accessToken = _accessTokenGenerator.Generate(user);
        var refreshToken = _refreshTokenGenerator.Generate();

        await _refreshTokenRepository.RevokeActiveTokensByUserIdAsync(user.Id, ct);
        await _refreshTokenRepository.AddAsync(new Domain.Entities.Auth.RefreshToken(user.Id, _refreshTokenGenerator.Hash(refreshToken), DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationDays)), ct);
        await _unitOfWork.Commit(ct);


        return new ResponseLoginJson
        {
            Name = user.Name,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
