using BookStore.Models;

namespace BookStore.Models
{
    /// <summary>
    /// Товар в корзине.
    /// </summary>
    public class CartItem
    {
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
