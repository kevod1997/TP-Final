using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Exceptions
{
    // Excepción específica para validaciones fallidas en órdenes
    public class OrderValidationException : OrderDomainException
    {
        public OrderValidationException(string message) : base(message)
        {
        }
    }
}