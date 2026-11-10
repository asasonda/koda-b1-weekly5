using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record TransferRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Wallet pengirim tidak valid.")]
    int SenderWalletId,

    [Range(1, int.MaxValue, ErrorMessage = "Silakan pilih wallet tujuan yang valid.")]
    [Required(ErrorMessage = "Receiver wajib diisi.")]
    int ReceiverWalletId,

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ErrorMessage = "Nominal transfer minimal Rp 0,01 dan tidak boleh melebihi batas maksimal."
    )]
    [Required(ErrorMessage = "Nominal transfer wajib diisi.")]
    decimal Amount,

    [StringLength(200, ErrorMessage = "Deskripsi maksimal 200 karakter.")]
    string? Description
);