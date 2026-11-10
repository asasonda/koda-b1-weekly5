using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public record UpdateUserRequest(
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama maksimal 50 karakter.")]
    string Name,

    [Required(ErrorMessage = "Email wajib diisi.")]
    [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    string Email,

    [Required(ErrorMessage = "Nomor telepon wajib diisi.")]
    [StringLength(20, ErrorMessage = "Nomor telepon maksimal 20 karakter.")]
    string Phone
);