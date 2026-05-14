using OnTimeScheduling.Domain.Entities.Auth;

namespace OnTimeScheduling.Application.Repositories.Auth;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);
    Task RevokeActiveTokensByUserIdAsync(Guid userId, CancellationToken ct);

}
