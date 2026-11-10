using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface ITransactionRepository
{
    Task<Transactions?> AddAsync(Transactions transactions, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transactions>> GetByUsersIdAsync(int usersId, CancellationToken cancellationToken);
}