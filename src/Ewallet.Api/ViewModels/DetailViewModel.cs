using Ewallet.Api.DTOs;

namespace Ewallet.Api.ViewModels;

public sealed class DetailViewModel
{
    public UserResponse User { get; set; } = null!;
    public WalletResponse Wallet { get; set; } = null!;
    public IReadOnlyList<TransactionResponse> Transactions { get; set; } = [];
}