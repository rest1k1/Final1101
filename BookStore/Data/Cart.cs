using BookStore.Models;

namespace BookStore.Data
{
    /// <summary>
    /// Хранит товары корзины.
    /// </summary>
    public static class Cart
    {
        public static List<CartItem> Items { get; set; } = new();
    }
}
