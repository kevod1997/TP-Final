using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Exceptions
{
    // Excepción específica para casos de stock insuficiente
    public class InsufficientStockException : ProductDomainException
    {
        public int ProductId { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
            : base($"Stock insuficiente para el producto {productId}. Requerido: {requestedQuantity}, Disponible: {availableQuantity}")
        {
            ProductId = productId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}