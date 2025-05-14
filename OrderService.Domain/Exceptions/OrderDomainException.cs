using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Exceptions
{
    // Excepción específica para problemas relacionados con órdenes
    public class OrderDomainException : DomainException
    {
        public OrderDomainException(string message) : base(message)
        {
        }
    }
}