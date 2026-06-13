using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Auth;
using OnTimeScheduling.Domain.Entities.Auth;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct)
        => await _context.RefreshTokens.AddAsync(refreshToken, ct);

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
        => _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public async Task RevokeActiveTokensByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var active = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in active)
            token.Revoke();
    }

}
