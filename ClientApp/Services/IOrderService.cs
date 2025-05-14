using ClientApp.Models;

namespace ClientApp.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<List<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto orderDto);
    }
}
