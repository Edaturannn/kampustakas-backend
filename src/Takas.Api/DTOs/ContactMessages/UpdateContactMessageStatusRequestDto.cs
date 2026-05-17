using System.ComponentModel.DataAnnotations;
using Takas.Api.Enums;

namespace Takas.Api.DTOs.ContactMessages;

public class UpdateContactMessageStatusRequestDto
{
    [Required(ErrorMessage = "Mesaj durumu zorunludur.")]
    public ContactMessageStatus? Status { get; set; }
}
