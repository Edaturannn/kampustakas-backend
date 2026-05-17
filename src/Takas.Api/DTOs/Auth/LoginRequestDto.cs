using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [MaxLength(200, ErrorMessage = "E-posta en fazla 200 karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    public string Password { get; set; } = string.Empty;
}
