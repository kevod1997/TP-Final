using ProductService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Entities
{
    public class Product
    {
        // Propiedades con acceso privado para setter para mantener la encapsulación
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Constructor privado para EF Core
        private Product() { }

        // Constructor para crear nuevos productos
        public Product(string name, string description, decimal price, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto no puede estar vacio", nameof(name));

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));

            if (stockQuantity < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(stockQuantity));

            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            StockQuantity = stockQuantity;
            CreatedAt = DateTime.UtcNow;
        }

        // Métodos para modificar la entidad
        public void UpdateDetails(string name, string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto no puede estar vacio", nameof(name));

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));

            Name = name;
            Description = description ?? string.Empty;
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            if (StockQuantity + quantity < 0)
                throw new InsufficientStockException(Id, -quantity, StockQuantity);

            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para verificar si hay suficiente stock
        public bool HasSufficientStock(int requestedQuantity)
        {
            return StockQuantity >= requestedQuantity;
        }
    }
}