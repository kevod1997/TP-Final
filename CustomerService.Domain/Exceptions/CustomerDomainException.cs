using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Domain.Exceptions
{
    // Excepción específica para problemas relacionados con clientes
    public class CustomerDomainException : DomainException
    {
        public CustomerDomainException(string message) : base(message)
        {
        }
    }
}