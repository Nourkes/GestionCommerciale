using GestionCommerciale.Domain.Entities;

namespace GestionCommerciale.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}