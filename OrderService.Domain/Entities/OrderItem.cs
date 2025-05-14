using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class OrderItem
    {
        // Propiedades con acceso privado para setter para mantener la encapsulación
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal Subtotal { get; private set; }

        // Constructor privado para EF Core
        private OrderItem() { }

        // Constructor para crear nuevos ítems de orden
        public OrderItem(int orderId, int productId, string productName, decimal unitPrice, int quantity)
        {
            if (orderId <= 0 && orderId != 0) // Permitimos 0 para órdenes nuevas que aún no se han guardado
                throw new ArgumentException("El ID de la orden debe ser mayor que cero", nameof(orderId));

            if (productId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor que cero", nameof(productId));

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(productName));

            if (unitPrice < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero", nameof(quantity));

            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Subtotal = unitPrice * quantity;
        }

        // Método para actualizar la cantidad del ítem
        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero", nameof(quantity));

            Quantity = quantity;
            Subtotal = UnitPrice * quantity;
        }
    }
}