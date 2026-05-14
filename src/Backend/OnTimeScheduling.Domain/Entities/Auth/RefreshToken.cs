using OnTimeScheduling.Domain.Entities.DefaultEntity;

namespace OnTimeScheduling.Domain.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public void Revoke()
    {
        if (RevokedAt is null)
            RevokedAt = DateTime.UtcNow;
    }

}
