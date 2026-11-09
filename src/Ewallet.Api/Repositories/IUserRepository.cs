using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken cancellationToken);
    Task<Users?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> AddUserWithWalletAsync(Users users, Wallets wallets, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(Users users, CancellationToken cancellationToken);
}