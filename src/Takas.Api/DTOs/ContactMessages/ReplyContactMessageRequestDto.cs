using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.ContactMessages;

public class ReplyContactMessageRequestDto
{
    [Required(ErrorMessage = "Admin yanıtı zorunludur.")]
    [MaxLength(4000, ErrorMessage = "Admin yanıtı en fazla 4000 karakter olabilir.")]
    public string AdminReply { get; set; } = string.Empty;
}
