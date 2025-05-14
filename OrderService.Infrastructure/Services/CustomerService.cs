using Microsoft.Extensions.Configuration;
using OrderService.Application.DTOs.External;
using OrderService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;


namespace OrderService.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILoggerService _logger;

        public CustomerService(HttpClient httpClient, IConfiguration configuration, ILoggerService logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ServiceUrls:CustomerService"] ?? "http://localhost:5002";
            _logger = logger;
        }

        public async Task<CustomerDto> GetCustomerAsync(int customerId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo datos del cliente con ID: {customerId} desde el servicio externo");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/customers/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
                    return customer;
                }

                _logger.LogWarning($"No se encontró el cliente con ID: {customerId}. Código de estado: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con ID: {customerId} desde el servicio externo");
                throw;
            }
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            try
            {
                _logger.LogInformation($"Verificando existencia del cliente con ID: {customerId}");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/customers/{customerId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar existencia del cliente con ID: {customerId}");
                return false;
            }
        }
    }
}