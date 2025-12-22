using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
    }
}
