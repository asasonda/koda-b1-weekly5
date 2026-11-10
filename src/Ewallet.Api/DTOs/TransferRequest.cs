using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record TransferRequest(
    [Range(1, int.MaxValue)]
    int SenderWalletId,

    [Required]
    int ReceiverWalletId,

    [Range(typeof(decimal), "0.01", "9999999999999999.99"), Required]
    decimal Amount,

    [StringLength(200)]
    string? Description
);