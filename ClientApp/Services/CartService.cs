using ClientApp.Models;

namespace ClientApp.Services
{
    public class CartService
    {
        public List<CartItem> Items { get; private set; } = new List<CartItem>();

        // Evento para notificar cambios en el carrito
        public event Action? OnChange;

        public void AddItem(ProductDto product, int quantity)
        {
            var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Limitar la cantidad al stock disponible
                int newQuantity = Math.Min(existingItem.Quantity + quantity, product.StockQuantity);
                existingItem.Quantity = newQuantity;
            }
            else
            {
                // Limitar la cantidad al stock disponible
                int limitedQuantity = Math.Min(quantity, product.StockQuantity);
                Items.Add(new CartItem { Product = product, Quantity = limitedQuantity });
            }

            NotifyStateChanged();
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                // Asegurar que la cantidad no exceda el stock disponible
                int limitedQuantity = Math.Min(quantity, item.Product.StockQuantity);
                item.Quantity = limitedQuantity > 0 ? limitedQuantity : 1; // Mínimo 1
                NotifyStateChanged();
            }
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                Items.Remove(item);
                NotifyStateChanged();
            }
        }

        public decimal GetTotal() => Items.Sum(i => i.Subtotal);

        public void Clear()
        {
            Items.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
