using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record UserRequest(
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nama maksimal 50 karakter.")]
    string Name,

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
    string Email,

    [Required(ErrorMessage = "Nomor telepon wajib diisi.")]
    [StringLength(20, ErrorMessage = "Nomor telepon maksimal 20 karakter.")]
    string Phone,

    [Required(ErrorMessage = "Nomor wallet wajib diisi.")]
    [StringLength(50, ErrorMessage = "Nomor wallet maksimal 50 karakter.")]
    string WalletNumber
);
