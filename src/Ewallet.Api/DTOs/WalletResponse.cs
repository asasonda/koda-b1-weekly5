using Ewallet.Api.Models;

namespace Ewallet.Api.DTOs;

public sealed record class WalletResponse(int Id,int UsersId, string WalletNumber, decimal Balance)
{
    public static WalletResponse FromModel(Wallets wallets) => new(wallets.Id, wallets.UsersId, wallets.WalletNumber, wallets.Balance);
}