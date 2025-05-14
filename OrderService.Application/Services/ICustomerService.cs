using OrderService.Application.DTOs.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerAsync(int customerId);
        Task<bool> CustomerExistsAsync(int customerId);
    }
}