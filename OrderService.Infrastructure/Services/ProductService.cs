using Microsoft.Extensions.Configuration;
using OrderService.Application.DTOs.External;
using OrderService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;

namespace OrderService.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILoggerService _logger;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ProductService(HttpClient httpClient, IConfiguration configuration, ILoggerService logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ServiceUrls:ProductService"] ?? "http://localhost:5001";
            _logger = logger;
        }

        public async Task<ProductDto> GetProductAsync(int productId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo datos del producto con ID: {productId} desde el servicio externo");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
                    return product;
                }

                _logger.LogWarning($"No se encontró el producto con ID: {productId}. Código de estado: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID: {productId} desde el servicio externo");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<int> productIds)
        {
            try
            {
                var products = new List<ProductDto>();

                // En un entorno de producción, esto podría mejorarse con una API de batch para obtener múltiples productos en una sola solicitud
                foreach (var productId in productIds)
                {
                    var product = await GetProductAsync(productId);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener múltiples productos desde el servicio externo");
                throw;
            }
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int quantity)
        {
            try
            {
                _logger.LogInformation($"Actualizando stock del producto con ID: {productId}, cantidad: {quantity}");

                var updateStockDto = new UpdateStockDto { Id = productId, Quantity = quantity };
                var content = new StringContent(
                    JsonSerializer.Serialize(updateStockDto, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                // Usar PatchAsync para coincidir con el endpoint [HttpPatch] del controlador
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_baseUrl}/api/products/{productId}/stock")
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Stock actualizado exitosamente para el producto con ID: {productId}");
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error al actualizar stock. Código de estado: {response.StatusCode}, Mensaje: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar stock del producto con ID: {productId}");
                throw;
            }
        }

        public async Task<bool> HasSufficientStockAsync(int productId, int requestedQuantity)
        {
            try
            {
                var product = await GetProductAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning($"No se puede verificar stock: Producto con ID {productId} no encontrado");
                    return false;
                }

                return product.StockQuantity >= requestedQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar stock del producto con ID: {productId}");
                return false;
            }
        }
    }
}