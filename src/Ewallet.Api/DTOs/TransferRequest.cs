using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record TransferRequest(
    [Range(1, int.MaxValue)]
    int SenderWalletId,

    [Required]
    string ReceiverWalletNumber,

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    decimal Amount
);