using ClientApp.Models;

namespace ClientApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:OrderApi"] ?? "https://localhost:5005");
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/orders");
                return response ?? new List<OrderDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return new List<OrderDto>();
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching order {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/orders/customer/{customerId}");
                return response ?? new List<OrderDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders for customer {customerId}: {ex.Message}");
                return new List<OrderDto>();
            }
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", orderDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                return null;
            }
        }
    }
}