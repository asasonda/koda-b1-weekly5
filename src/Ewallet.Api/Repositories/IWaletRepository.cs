using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface IWalletRepository
{
    Task<IReadOnlyList<Wallets>> GetAllAsync(CancellationToken cancellationToken);
    Task<Wallets?> GetByIdUsers(int usersId, CancellationToken cancellationToken);

    
    Task<bool> TopUpAddAsync(int usersId, decimal amount, CancellationToken cancellationToken);
    

    Task<bool> ReduceBalanceAsync(int id, decimal amount, CancellationToken cancellationToken);
    Task<bool> AddBalanceAsync(int id, decimal amount, CancellationToken cancellationToken);
}