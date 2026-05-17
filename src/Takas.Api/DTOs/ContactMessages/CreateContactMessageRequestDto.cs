using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.ContactMessages;

public class CreateContactMessageRequestDto
{
    [Required(ErrorMessage = "Mesaj konusu zorunludur.")]
    [MaxLength(200, ErrorMessage = "Mesaj konusu en fazla 200 karakter olabilir.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj içeriği zorunludur.")]
    [MaxLength(4000, ErrorMessage = "Mesaj içeriği en fazla 4000 karakter olabilir.")]
    public string Message { get; set; } = string.Empty;
}
