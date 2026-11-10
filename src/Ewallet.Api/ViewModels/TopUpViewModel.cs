using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.ViewModels;

public class TopUpViewModel
{
    [Required]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
}