namespace ClientApp.Models
{
    public class CartItem
    {
        public ProductDto Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Subtotal => Product.Price * Quantity;
    }
}