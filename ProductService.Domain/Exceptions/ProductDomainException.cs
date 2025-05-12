using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    // Excepción específica para problemas relacionados con productos
    public class ProductDomainException : DomainException
    {
        public ProductDomainException(string message) : base(message)
        {
        }
    }
}
