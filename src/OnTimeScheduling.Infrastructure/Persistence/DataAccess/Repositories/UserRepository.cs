using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

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
        email = email.Trim();

        return _db.Users.AsNoTracking().AnyAsync(u => u.Email == email && u.Status == RecordStatus.Active, ct);
    }

    public async Task<User?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
    }
}
