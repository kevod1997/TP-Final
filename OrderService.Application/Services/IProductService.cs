using OrderService.Application.DTOs.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductAsync(int productId);
        Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<int> productIds);
        Task<bool> UpdateProductStockAsync(int productId, int quantity);
        Task<bool> HasSufficientStockAsync(int productId, int requestedQuantity);
    }
}