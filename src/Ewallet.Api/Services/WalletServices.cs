using Ewallet.Api.DTOs;
using Ewallet.Api.Repositories;

namespace Ewallet.Api.Services;

public sealed class WalletServices : IWalletServices
{
    private readonly IWalletRepository _repository;

    public WalletServices(IWalletRepository repository)
    {
        _repository = repository;
    }
    public async Task<WalletResponse?> GetByIdUsersAsync(int usersId, CancellationToken cancellationToken)
    {
        var wallets = await _repository.GetByIdUsers(usersId, cancellationToken);
        return wallets is null ? null : WalletResponse.FromModel(wallets);
    }
}