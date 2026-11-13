using GestionCommerciale.Application.Interfaces;
using GestionCommerciale.Domain.Entities;
using GestionCommerciale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}