using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public record UpdateUserRequest(
    [Required, StringLength(50)]
    string Name,

    [Required, StringLength(100)]
    string Email,

    [Required, StringLength(255)]
    string Password,

    [Required, StringLength(20)]
    string Phone
);