using OrderService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        // Propiedades con acceso privado para setter para mantener la encapsulación
        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public DateTime? UpdatedAt { get; private set; }

        // Constructor privado para EF Core
        private Order() { }

        // Constructor para crear nuevas órdenes
        public Order(int customerId, string customerName)
        {
            if (customerId <= 0)
                throw new ArgumentException("El ID del cliente debe ser mayor que cero", nameof(customerId));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("El nombre del cliente no puede estar vacío", nameof(customerName));

            CustomerId = customerId;
            CustomerName = customerName;
            OrderDate = DateTime.UtcNow;
            TotalAmount = 0;
        }

        // Método para agregar un ítem a la orden
        public void AddItem(int productId, string productName, decimal unitPrice, int quantity)
        {
            if (productId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor que cero", nameof(productId));

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(productName));

            if (unitPrice < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero", nameof(quantity));

            // Verificar si el producto ya existe en la orden
            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                // Si el producto ya existe, actualizar la cantidad
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                // Si el producto no existe, agregar un nuevo ítem
                var orderItem = new OrderItem(this.Id, productId, productName, unitPrice, quantity);
                _items.Add(orderItem);
            }

            // Recalcular el total de la orden
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para actualizar la cantidad de un ítem existente
        public void UpdateItemQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero", nameof(quantity));

            var item = _items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new OrderDomainException($"No se encontró un ítem con el ID de producto {productId} en la orden");

            item.UpdateQuantity(quantity);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para eliminar un ítem de la orden
        public void RemoveItem(int productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new OrderDomainException($"No se encontró un ítem con el ID de producto {productId} en la orden");

            _items.Remove(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        // Método privado para recalcular el total de la orden
        private void RecalculateTotal()
        {
            TotalAmount = _items.Sum(i => i.Subtotal);
        }
    }
}