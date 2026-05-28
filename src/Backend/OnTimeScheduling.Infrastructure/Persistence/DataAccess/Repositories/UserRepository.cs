using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Domain.Extensions;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task Add(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task<bool> EmailExists(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.SanitizeEmail();

        return _db.Users.AsNoTracking().AnyAsync(u => u.Email == normalizedEmail, ct);
    }
    public Task<bool> EmailExistsExceptId(string email, Guid userId, CancellationToken ct = default)
    {
        var normalizedEmail = email.SanitizeEmail();

        return _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail && u.Id != userId, ct);
    }

    public async Task<User?> GetByEmail(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.SanitizeEmail();

        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.Status == RecordStatus.Active, ct);
    }

    public async Task<User?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByIdAndCompany(Guid id, Guid companyId, CancellationToken ct = default)
    {
        return await _db.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && x.Status == RecordStatus.Active, ct);
    }
    public async Task<User?> GetByIdAndCompanyIncludingInactive(Guid id, Guid companyId, CancellationToken ct = default)
    {
        return await _db.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId, ct);
    }

    public async Task<(List<User> Items, int TotalItems)> GetCompanyUsers(Guid companyId, int skip, int take, UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _db.Users
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId && user.Role != UserRole.SUPER_ADMIN);

        if (role.HasValue)
            query = query.Where(user => user.Role == role.Value);

        if (status.HasValue)
            query = query.Where(user => user.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(user =>
                user.Name.ToLower().Contains(normalizedSearchTerm) ||
                user.Email.ToLower().Contains(normalizedSearchTerm));
        }

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);

    }


    public void Update(User user)
    {
        _db.Users.Update(user);
    }
}
