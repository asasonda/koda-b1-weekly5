namespace Ewallet.Api.DTOs;

public sealed record TransactionResponse(
    int Id,
    int? SenderWalletId,
    int ReceiverWalletId,
    string Type,
    decimal Amount,
    string? Description,
    DateTime CreatedAt
);