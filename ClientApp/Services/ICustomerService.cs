using ClientApp.Models;

namespace ClientApp.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto customerDto);
    }
}
