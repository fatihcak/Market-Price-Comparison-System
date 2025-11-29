using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByUsernameAsync(string username);
    Task AddAsync(AdminUser adminUser);
    Task<bool> ExistsAsync(string username);
    Task SaveChangesAsync();
}
