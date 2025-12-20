using Domain.Entities;
using Domain.Interfaces.Repositories;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _context;

    public AdminUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username)
    {
        return await _context.AdminUser.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(AdminUser adminUser)
    {
        await _context.AdminUser.AddAsync(adminUser);
    }

    public async Task<bool> ExistsAsync(string username)
    {
        return await _context.AdminUser.AnyAsync(u => u.Username == username);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
