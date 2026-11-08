using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public record UserRequest(
    [Required, StringLength(50)]
    string Name,

    [Required, StringLength(100)]
    string Email,

    [Required]
    string Password,

    [Required, StringLength(20)]
    string Phone,

    [Required, StringLength(50)]
    string WalletNumber,

    [Required, StringLength(8)]
    string Pin
);