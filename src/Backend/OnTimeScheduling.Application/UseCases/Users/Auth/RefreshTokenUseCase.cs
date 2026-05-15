using OnTimeScheduling.Application.Repositories.Auth;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenSettings _settings;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenUseCase(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository,
        IAccessTokenGenerator accessTokenGenerator, IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenSettings settings, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _settings = settings;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseLoginJson> ExecuteAsync(RequestRefreshTokenJson request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new InvalidLoginException("Invalid refresh token.");

        var hash = _refreshTokenGenerator.Hash(request.RefreshToken);
        var stored = await _refreshTokenRepository.GetByTokenHashAsync(hash, ct);

        if (stored is null || !stored.IsActive)
            throw new InvalidLoginException("Invalid refresh token.");

        var user = await _userRepository.GetById(stored.UserId, ct);
        if (user is null)
            throw new InvalidLoginException("Invalid refresh token.");

        stored.Revoke();

        var newRefreshPlain = _refreshTokenGenerator.Generate();
        var newRefresh = new Domain.Entities.Auth.RefreshToken(user.Id, _refreshTokenGenerator.Hash(newRefreshPlain), DateTime.UtcNow.AddDays(_settings.ExpirationDays));

        await _refreshTokenRepository.AddAsync(newRefresh, ct);
        await _unitOfWork.Commit(ct);

        return new ResponseLoginJson { Name = user.Name, AccessToken = _accessTokenGenerator.Generate(user), RefreshToken = newRefreshPlain };
    }

}
