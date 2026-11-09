using Ewallet.Api.DTOs;

namespace Ewallet.Api.Services;

public interface IWalletServices
{
    Task<WalletResponse?> GetByIdUsersAsync(int usersId, CancellationToken cancellationToken);
}