using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Ad soyad en fazla 200 karakter olabilir.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [MaxLength(200, ErrorMessage = "E-posta en fazla 200 karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}
