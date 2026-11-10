
using Ewallet.Api.Models;

namespace Ewallet.Api.DTOs;

public sealed record TransactionResponse(int Id, int? SenderWalletId, int ReceiverWalletId, string Type, decimal Amount, string? Description, DateTime CreatedAt)
{
    public static TransactionResponse FromModel(Transactions transaction) =>
        new(
            transaction.Id,
            transaction.SenderWalletId,
            transaction.ReceiverWalletId,
            transaction.Type,
            transaction.Amount,
            transaction.Description,
            transaction.CreatedAt
        );
}