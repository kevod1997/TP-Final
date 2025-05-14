using ClientApp.Models;

namespace ClientApp.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CustomerService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:CustomerApi"] ?? "https://localhost:5003");
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers");
                return response ?? new List<CustomerDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers: {ex.Message}");
                return new List<CustomerDto>();
            }
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto customerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/customers", customerDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CustomerDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return null;
            }
        }
    }
}