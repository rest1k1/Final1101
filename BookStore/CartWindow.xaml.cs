using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    /// <summary>
    /// Окно корзины.
    /// </summary>
    public partial class CartWindow : Window
    {
        private readonly OrderService orderService = new();

        public CartWindow()
        {
            InitializeComponent();

            OrdersGrid.ItemsSource = Cart.Items;

            UpdateTotal();
        }

        /// <summary>
        /// Удаление товара из корзины.
        /// </summary>
        private void Delete_Click(
            object sender,
            RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show(
                "Удалить товар из корзины?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            CartItem? selectedItem =
                (sender as Button)?.DataContext as CartItem;

            if (selectedItem == null)
            {
                return;
            }

            Cart.Items.Remove(selectedItem);

            OrdersGrid.Items.Refresh();

            UpdateTotal();

            if (LoginWindow.CurrentUser != null)
            {
                CartStorageService.SaveCart();
            }

            if (Cart.Items.Count == 0)
            {
                Close();
            }

            if (Application.Current.MainWindow
                is MainWindow mainWindow)
            {
                mainWindow.UpdateOrdersButton();
                mainWindow.UpdateCartButton();
            }
        }

        /// <summary>
        /// Оформляет заказ.
        /// </summary>
        private void Checkout_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                using var context = new AppDbContext();

                string? clientName = null;

                if (LoginWindow.CurrentUser != null)
                {
                    clientName =
                        LoginWindow.CurrentUser.FullName;
                }

                int orderNumber =
                    orderService.GetNextOrderNumber();

                Random random = new();

                int receiveCode =
                    random.Next(100, 1000);

                string articles =
                    string.Join(", ",
                        Cart.Items.Select(item =>
                            $"{item.Product.Article}, {item.Quantity}"));

                Order order = new Order
                {
                    OrderNumber = orderNumber,

                    Article = articles,

                    OrderDate = DateTime.Now
                        .ToString("dd.MM.yyyy HH:mm:ss"),

                    DeliveryDate = DateTime.Now
                        .AddDays(3)
                        .ToString("dd.MM.yyyy"),

                    Status = "Новый",

                    ClientName = clientName,

                    CodeForReceive = receiveCode
                };

                context.Orders.Add(order);

                context.SaveChanges();

                foreach (var item in Cart.Items)
                {
                    OrderItem orderItem = new OrderItem
                    {
                        OrderId = order.Id,

                        ProductId = item.Product.Id,

                        Quantity = item.Quantity
                    };

                    context.OrderItems.Add(orderItem);
                }

                context.SaveChanges();

                SaveReceiptToFile(
                    orderNumber,
                    clientName,
                    receiveCode);

                MessageBox.Show(
                    $"Заказ №{orderNumber} оформлен",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Cart.Items.Clear();

                CartStorageService.ClearCart();

                OrdersGrid.Items.Refresh();

                UpdateTotal();

                if (Application.Current.MainWindow
                    is MainWindow mainWindow)
                {
                    mainWindow.UpdateOrdersButton();
                    mainWindow.UpdateCartButton();
                }

                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Ошибка оформления заказа.\n{exception.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновление итоговой суммы.
        /// </summary>
        private void UpdateTotal()
        {
            decimal total =
                Cart.Items.Sum(item =>
                    (decimal)item.Product.Price
                    * item.Quantity);

            TotalText.Text =
                $"Сумма заказа: {total} руб.";
        }

        /// <summary>
        /// Сохраняет талон заказа в текстовый файл.
        /// </summary>
        private void SaveReceiptToFile(
            int orderNumber,
            string? clientName,
            int receiveCode)
        {
            try
            {
                SaveFileDialog dialog =
                    new SaveFileDialog
                    {
                        Filter =
                            "Text file (*.txt)|*.txt",

                        FileName =
                            $"Order_{orderNumber}.txt"
                    };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                StringBuilder stringbuilder =
                    new StringBuilder();

                stringbuilder.AppendLine(
                    "=== ТАЛОН ЗАКАЗА ===");

                stringbuilder.AppendLine(
                    $"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                stringbuilder.AppendLine(
                    $"Номер заказа: {orderNumber}");

                stringbuilder.AppendLine(
                    $"Код получения: {receiveCode}");


                stringbuilder.AppendLine();

                stringbuilder.AppendLine("СОСТАВ ЗАКАЗА:");

                foreach (var item in Cart.Items)
                {
                    stringbuilder.AppendLine(
                        $"{item.Product.Name} x{item.Quantity} = " +
                        $"{item.Product.Price * item.Quantity} руб.");
                }

                decimal total =
                    Cart.Items.Sum(item =>
                        (decimal)item.Product.Price
                        * item.Quantity);

                stringbuilder.AppendLine();

                stringbuilder.AppendLine(
                    $"ИТОГО: {total} руб.");

                File.WriteAllText(
                    dialog.FileName,
                    stringbuilder.ToString());

                MessageBox.Show(
                    "Талон заказа успешно сохранён.",
                    "Сохранение файла",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Ошибка сохранения файла.\n{exception.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void Back_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}
