using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace BookStore
{
    /// <summary>
    /// Окно управления заказами.
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private readonly AppDbContext _database = new();

        public OrdersWindow()
        {
            InitializeComponent();

            LoadOrders();
        }

        private List<Order> allOrders = new();

        /// <summary>
        /// Загрузка заказов.
        /// </summary>
        private void LoadOrders()
        {
            try
            {
                allOrders =
                    _database.Orders.ToList();

                OrdersGrid.ItemsSource = allOrders;
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Ошибка загрузки заказов.\n{exception.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Сохранение изменений заказов.
        /// </summary>
        private void SaveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                _database.SaveChanges();

                MessageBox.Show(
                    "Изменения сохранены",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Ошибка сохранения.\n{exception.Message}",
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

        /// <summary>
        /// Выполняет поиск заказа по его номеру.
        /// </summary>
        private void SearchButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            string search = SearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                MessageBox.Show(
                    "Необходимо ввести номер заказа.",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(search, out _))
            {
                MessageBox.Show(
                    "Номер заказа должен содержать только цифры.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            List<Order> foundOrders =
                allOrders.Where(order =>
                    order.OrderNumber.ToString() == search)
                .ToList();

            if (foundOrders.Count == 0)
            {
                MessageBox.Show(
                    "Заказ с таким номером не найден.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            OrdersGrid.ItemsSource = foundOrders;
        }

        /// <summary>
        /// Сбрасывает результаты поиска и отображает все заказы.
        /// </summary>
        private void ResetButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchBox.Clear();

            OrdersGrid.ItemsSource = allOrders;
        }
    }
}
