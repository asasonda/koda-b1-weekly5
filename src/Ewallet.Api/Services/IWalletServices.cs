using Ewallet.Api.DTOs;

namespace Ewallet.Api.Services;

public interface IWalletServices
{
    Task<WalletResponse?> GetByIdUsersAsync(int usersId, CancellationToken cancellationToken);
    Task<TransactionResponse?> TopUpAsync(int usersId, TopUpRequest request, CancellationToken cancellationToken);

    Task<TransactionResponse?> TransferAsync(TransferRequest request, CancellationToken cancellationToken);
}