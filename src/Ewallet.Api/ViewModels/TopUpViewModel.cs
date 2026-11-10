using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.ViewModels;

public class TopUpViewModel
{
    [Required(ErrorMessage = "Nominal top up wajib diisi.")]
    [Range(
        typeof(decimal),
        "1",
        "999999999999",
        ErrorMessage = "Nominal top up minimal Rp 1 dan maksimal Rp 999.999.999.999."
    )]
    public decimal Amount { get; set; }

    [StringLength(200, ErrorMessage = "Deskripsi maksimal 200 karakter.")]
    public string? Description { get; set; }
}