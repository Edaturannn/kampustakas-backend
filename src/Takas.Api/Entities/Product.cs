using Takas.Api.Enums;

namespace Takas.Api.Entities;

public class Product
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal EstimatedMinPrice { get; set; }
    public decimal EstimatedMaxPrice { get; set; }
    public string Campus { get; set; } = string.Empty;
    public ProductStatus Status { get; set; } = ProductStatus.Available;
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Owner { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
