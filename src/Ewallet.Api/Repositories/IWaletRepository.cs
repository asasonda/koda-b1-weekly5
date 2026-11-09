using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface IWalletRepository
{
    Task<IReadOnlyList<Wallets>> GetAllAsync(CancellationToken cancellationToken);
    Task<Wallets?> GetByIdUsers(int usersId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Wallets wallets, CancellationToken cancellationToken);
}