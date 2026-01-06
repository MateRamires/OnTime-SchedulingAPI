using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.Repositories.Users;

public interface IUserRepository
{
    Task Add(User user, CancellationToken ct = default);
    Task<User?> GetById(Guid id, CancellationToken ct = default);
    Task<bool> EmailExists(string email, CancellationToken ct = default);
    void Update(User user);
}
