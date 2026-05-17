using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Categories;

public class CreateCategoryRequestDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;
}
