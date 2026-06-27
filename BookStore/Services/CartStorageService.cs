using BookStore.Data;
using BookStore.Models;
using System.Text.Json;
using System.IO;

namespace BookStore.Services
{
    /// <summary>
    /// Сервис сохранения корзины пользователя.
    /// </summary>
    public static class CartStorageService
    {
        /// <summary>
        /// Получает путь файла корзины.
        /// </summary>
        private static string GetCartFilePath()
        {
            string login =
                LoginWindow.CurrentUser?.Login ?? "guest";

            return $"cart_{login}.json";
        }

        /// <summary>
        /// Сохраняет корзину.
        /// </summary>
        public static void SaveCart()
        {
            try
            {
                string json =
                    JsonSerializer.Serialize(Cart.Items);

                File.WriteAllText(
                    GetCartFilePath(),
                    json);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Загружает корзину.
        /// </summary>
        public static void LoadCart()
        {
            try
            {
                Cart.Items.Clear();

                string path = GetCartFilePath();

                if (!File.Exists(path))
                {
                    return;
                }

                string json =
                    File.ReadAllText(path);

                List<CartItem>? items =
                    JsonSerializer.Deserialize<List<CartItem>>(json);

                if (items == null)
                {
                    return;
                }

                foreach (CartItem item in items)
                {
                    Cart.Items.Add(item);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Очищает корзину.
        /// </summary>
        public static void ClearCart()
        {
            string path = GetCartFilePath();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
