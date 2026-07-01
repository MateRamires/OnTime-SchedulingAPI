using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;

namespace OnTimeScheduling.Infrastructure.Security.Concurrency;

public sealed class PostgresAgendaConcurrencyGuard : IAgendaConcurrencyGuard
{
    private readonly AppDbContext _dbContext;
    private readonly ITenantProvider _tenantProvider;

    public PostgresAgendaConcurrencyGuard(
        AppDbContext dbContext,
        ITenantProvider tenantProvider)
    {
        _dbContext = dbContext;
        _tenantProvider = tenantProvider;
    }

    public async Task ExecuteAsync(
        IEnumerable<AgendaConcurrencyLockKey> lockKeys,
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default)
    {
        await ExecuteAsync<object?>(
            lockKeys,
            async lockedCt =>
            {
                await operation(lockedCt);
                return null;
            },
            ct);
    }

    public async Task<T> ExecuteAsync<T>(
        IEnumerable<AgendaConcurrencyLockKey> lockKeys,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken ct = default)
    {
        var companyId = _tenantProvider.CompanyId;
        if (!companyId.HasValue)
            return await operation(ct);

        var normalizedLockKeys = lockKeys
            .Distinct()
            .OrderBy(lockKey => lockKey.ResourceType, StringComparer.Ordinal)
            .ThenBy(lockKey => lockKey.ResourceId)
            .ToList();

        var ownsTransaction = _dbContext.Database.CurrentTransaction is null;
        await using var transaction = ownsTransaction
            ? await _dbContext.Database.BeginTransactionAsync(ct)
            : null;

        try
        {
            foreach (var lockKey in normalizedLockKeys)
                await AcquireLockAsync(companyId.Value, lockKey, ct);

            var result = await operation(ct);

            if (ownsTransaction && transaction is not null)
                await transaction.CommitAsync(ct);

            return result;
        }
        catch
        {
            if (ownsTransaction && transaction is not null)
                await transaction.RollbackAsync(CancellationToken.None);

            throw;
        }
    }

    private async Task AcquireLockAsync(
        Guid companyId,
        AgendaConcurrencyLockKey lockKey,
        CancellationToken ct)
    {
        var lockId = CreateStableLockId(companyId, lockKey);
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock({lockId})",
            ct);
    }

    private static long CreateStableLockId(Guid companyId, AgendaConcurrencyLockKey lockKey)
    {
        var value = $"{companyId:N}:{lockKey.ResourceType}:{lockKey.ResourceId:N}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        return BitConverter.ToInt64(hash, 0);
    }
}
