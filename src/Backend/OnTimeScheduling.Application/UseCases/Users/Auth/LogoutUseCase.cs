using OnTimeScheduling.Application.Repositories.Auth;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public class LogoutUseCase : ILogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository, IRefreshTokenGenerator refreshTokenGenerator, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(RequestLogoutJson request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        var tokenHash = _refreshTokenGenerator.Hash(request.RefreshToken);
        var stored = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);
        if (stored is not null && stored.IsActive)
        {
            stored.Revoke();
            await _unitOfWork.Commit(ct);
        }
    }

}
