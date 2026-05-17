using Takas.Api.DTOs.Products;

namespace Takas.Api.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ProductResponseDto>> GetMyProductsAsync(CancellationToken cancellationToken = default);
    Task<List<ProductResponseDto>> GetProductsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
