using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Users;

public interface IUserRepository
{
    Task Add(User user, CancellationToken ct = default);
    Task<User?> GetById(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdAndCompany(Guid id, Guid companyId, CancellationToken ct = default);
    Task<User?> GetByIdAndCompanyIncludingInactive(Guid id, Guid companyId, CancellationToken ct = default);
    Task<User?> GetByEmail(string email, CancellationToken ct = default);
    Task<(List<User> Items, int TotalItems)> GetCompanyUsers(Guid companyId, int skip, int take, UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
    Task<bool> EmailExists(string email, CancellationToken ct = default);
    Task<bool> EmailExistsExceptId(string email, Guid userId, CancellationToken ct = default);
    void Update(User user);
}
