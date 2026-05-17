using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Messages;

public class SendMessageRequestDto
{
    [Required(ErrorMessage = "Mesaj alıcısı zorunludur.")]
    public int ReceiverId { get; set; }

    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Mesaj içeriği zorunludur.")]
    [MaxLength(2000, ErrorMessage = "Mesaj içeriği en fazla 2000 karakter olabilir.")]
    public string Content { get; set; } = string.Empty;
}
