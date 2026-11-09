using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record UserRequest(
    [Required, StringLength(50)]
    string Name,

    [Required, EmailAddress, StringLength(100)]
    string Email,

    [Required, StringLength(20)]
    string Phone,

    [Required, StringLength(50)]
    string WalletNumber
);
