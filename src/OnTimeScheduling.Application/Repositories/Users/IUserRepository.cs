using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.Repositories.Users;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Update(User user);
}
