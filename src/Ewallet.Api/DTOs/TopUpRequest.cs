using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record TopUpRequest(
    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ErrorMessage = "Nominal transfer minimal Rp 0,01."
    )]
    [Required(ErrorMessage = "Nominal transfer wajib diisi.")]
    decimal Amount
);