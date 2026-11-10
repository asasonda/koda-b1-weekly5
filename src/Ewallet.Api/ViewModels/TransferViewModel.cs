using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ewallet.Api.ViewModels;

public class TransferViewModel
{
    [Required(ErrorMessage = "Pilih wallet tujuan.")]
    public int? ReceiverWalletId { get; set; }

    [Range(typeof(decimal), "1", "999999999999", ErrorMessage = "Nominal transfer harus lebih dari 0.")]
    public decimal Amount { get; set; }

    [StringLength(200,ErrorMessage = "Deskripsi maksimal 200 karakter.")]
    public string? Description { get; set; }

    public List<SelectListItem> WalletOptions { get; set; } = [];
}