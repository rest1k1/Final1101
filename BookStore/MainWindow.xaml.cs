using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    /// <summary>
    /// Главное окно приложения.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductService _productService = new();

        private List<Product> allProducts = new();

        public MainWindow()
        {
            InitializeComponent();

            UserNameText.Text = "Гость";
        }

        /// <summary>
        /// Инициализация информации о пользователе.
        /// </summary>
        private void InitializeUser()
        {
            if (LoginWindow.CurrentUser == null)
            {
                return;
            }

            UserNameText.Text =
                LoginWindow.CurrentUser.FullName;

            bool hasAccess =
                LoginWindow.CurrentUser.Role == "Администратор" ||
                LoginWindow.CurrentUser.Role == "Менеджер";

            OrdersButton.Visibility =
                hasAccess
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        /// <summary>
        /// Загрузка данных при открытии окна.
        /// </summary>
        private void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                string? savedLogin =
                    SessionService.LoadSession();

                if (!string.IsNullOrWhiteSpace(savedLogin))
                {
                    using AppDbContext context = new();

                    LoginWindow.CurrentUser =
                        context.Users.FirstOrDefault(user =>
                            user.Login == savedLogin);

                    if (LoginWindow.CurrentUser != null)
                    {
                        UserNameText.Text =
                            LoginWindow.CurrentUser.FullName;

                        LoginButton.Visibility =
                            Visibility.Collapsed;

                        LogoutButton.Visibility =
                            Visibility.Visible;

                        InitializeUser();

                        CartStorageService.LoadCart();

                        UpdateCartButton();
                    }
                }
                else
                {
                    Cart.Items.Clear();
                }

                CartStorageService.LoadCart();

                allProducts = _productService.GetProducts();

                SortBox.SelectedIndex = 0;

                ProductsList.ItemsSource = allProducts;

                ManufacturerBox.Items.Clear();

                ManufacturerBox.Items.Add(
                    "Все производители");

                foreach (string manufacturer in allProducts
                    .Select(product => product.Manufacturer)
                    .Distinct())
                {
                    ManufacturerBox.Items.Add(manufacturer);
                }

                ManufacturerBox.SelectedIndex = 0;

                UpdateProducts();

                UpdateOrdersButton();

                UpdateCartButton();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Ошибка загрузки товаров.\n{exception.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет список товаров.
        /// </summary>
        private void UpdateProducts()
        {
            if (ProductsList == null || allProducts == null)
            {
                return;
            }

            IEnumerable<Product> products = allProducts;

            products = ApplySearch(products);

            products = ApplyManufacturerFilter(products);

            if (!ValidatePrices())
            {
                return;
            }

            products = ApplyPriceFilter(products);

            products = ApplySorting(products);

            ShowProducts(products);
        }

        /// <summary>
        /// Выполняет поиск товаров.
        /// </summary>
        private IEnumerable<Product> ApplySearch(
            IEnumerable<Product> products)
        {
            string searchText = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return products;
            }

            return products.Where(product =>
                product.Name.ToLower().Contains(searchText));
        }
        /// <summary>
        /// Фильтрация по производителю.
        /// </summary>
        private IEnumerable<Product> ApplyManufacturerFilter(
            IEnumerable<Product> products)
        {
            if (ManufacturerBox.SelectedItem == null)
            {
                return products;
            }

            string manufacturer =
                ManufacturerBox.SelectedItem.ToString();

            if (manufacturer == "Все производители")
            {
                return products;
            }

            return products.Where(product =>
                product.Manufacturer == manufacturer);
        }

        /// <summary>
        /// Фильтрация по цене.
        /// </summary>
        private IEnumerable<Product> ApplyPriceFilter(
            IEnumerable<Product> products)
        {
            if (double.TryParse(
                PriceFromBox.Text,
                out double minPrice))
            {
                products = products.Where(product =>
                    product.Price >= minPrice);
            }

            if (double.TryParse(
                PriceToBox.Text,
                out double maxPrice))
            {
                products = products.Where(product =>
                    product.Price <= maxPrice);
            }

            return products;
        }

        /// <summary>
        /// Проверка корректности цены.
        /// </summary>
        private bool ValidatePrices()
        {
            if (double.TryParse(PriceFromBox.Text, out double min))
            {
                if (min < 0)
                {
                    MessageBox.Show(
                        "Цена не может быть отрицательной",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    PriceFromBox.Clear();

                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(PriceFromBox.Text) &&
                !double.TryParse(PriceFromBox.Text, out _))
            {
                MessageBox.Show(
                    "Минимальная цена введена неверно",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                PriceFromBox.Clear();

                return false;
            }

            if (!string.IsNullOrWhiteSpace(PriceToBox.Text) &&
                !double.TryParse(PriceToBox.Text, out _))
            {
                MessageBox.Show(
                    "Максимальная цена введена неверно",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                PriceToBox.Clear();

                return false;
            }

            return true;
        }



        /// <summary>
        /// Сортировка товаров.
        /// </summary>
        private IEnumerable<Product> ApplySorting(
            IEnumerable<Product> products)
        {
            if (SortBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return products;
            }

            switch (selectedItem.Content.ToString())
            {
                case "Название (А-Я)":
                    return products.OrderBy(product =>
                        product.Name);

                case "Название (Я-А)":
                    return products.OrderByDescending(product =>
                        product.Name);

                case "Цена (по возрастанию)":
                    return products.OrderBy(product =>
                        product.Price);

                case "Цена (по убыванию)":
                    return products.OrderByDescending(product =>
                        product.Price);

                default:
                    return products.OrderBy(product =>
                        product.Id);
            }
        }

        /// <summary>
        /// Отображение товаров.
        /// </summary>
        private void ShowProducts(
            IEnumerable<Product> products)
        {
            List<Product> filteredProducts =
                products.ToList();

            ProductsList.ItemsSource = filteredProducts;

            if (filteredProducts.Count == 0)
            {
                CountText.Text = "Товары не найдены";

                EmptyText.Visibility = Visibility.Visible;
                ProductsList.Visibility = Visibility.Collapsed;
            }
            else
            {
                CountText.Text =
                    $"{filteredProducts.Count} из {allProducts.Count}";

                EmptyText.Visibility = Visibility.Collapsed;

                ProductsList.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Выполняет поиск товаров.
        /// </summary>
        private void SearchBox_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ManufacturerBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        /// <summary>
        /// Выполняет выход из аккаунта.
        /// </summary>
        private void Logout_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите выйти?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            SessionService.ClearSession();

            LoginWindow.CurrentUser = null;

            Cart.Items.Clear();

            CartStorageService.ClearCart();

            UserNameText.Text = "Гость";

            LoginButton.Visibility =
                Visibility.Visible;

            LogoutButton.Visibility =
                Visibility.Collapsed;

            UpdateCartButton();

            UpdateOrdersButton();

            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Collapsed;
            OrdersButton.Visibility = Visibility.Collapsed;

            MessageBox.Show(
                "Вы вышли из аккаунта",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Открывает окно авторизации.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();

            if (loginWindow.ShowDialog() == true)
            {
                UserNameText.Text = LoginWindow.CurrentUser.FullName;

                LoginButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;

                InitializeUser();

                MessageBox.Show(
                    $"Добро пожаловать, {LoginWindow.CurrentUser.FullName}!",
                    "Авторизация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

            }
            UpdateOrdersButton();
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки добавления товара в корзину.
        /// Если товар уже присутствует в корзине, увеличивает его количество,
        /// иначе добавляет новый элемент. После этого обновляет интерфейс
        /// и сохраняет корзину для авторизованного пользователя.
        /// </summary>
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            Button orderButton = (Button)sender;

            Product product = (Product)orderButton.Tag;

            CartItem existingItem =
                Cart.Items.FirstOrDefault(item =>
                    item.Product.Id == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                Cart.Items.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1
                });
            }
            MessageBox.Show(
                $"Добавлено в корзину:\n{product.Name}\nЦена: {product.Price} руб.",
                "Корзина",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            UpdateCartButton();
            UpdateOrdersButton();

            if (LoginWindow.CurrentUser != null)
            {
                CartStorageService.SaveCart();
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new();

            cartWindow.ShowDialog();

            UpdateCartButton();
            UpdateOrdersButton();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new();

            ordersWindow.ShowDialog();

            UpdateCartButton();
            UpdateOrdersButton();
        }

        /// <summary>
        /// Обновляет отображение кнопки заказов.
        /// </summary>
        public void UpdateOrdersButton()
        {
            if (LoginWindow.CurrentUser == null)
            {
                OrdersButton.Visibility = Visibility.Collapsed;
                return;
            }

            bool hasAccess =
                LoginWindow.CurrentUser.Role == "Администратор" ||
                LoginWindow.CurrentUser.Role == "Менеджер";

            OrdersButton.Visibility =
                hasAccess
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        /// <summary>
        /// Обновляет отображение кнопки корзины.
        /// </summary>
        public void UpdateCartButton()
        {
            CartButton.Visibility =
                Cart.Items.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        /// <summary>
        /// Проверяет корректность введённой цены после потери полем ввода фокуса.
        /// Если значение не является числом, выводит сообщение об ошибке
        /// и очищает поле ввода.
        /// </summary>
        private void PriceBox_LostFocus(
            object sender,
            RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrWhiteSpace(textBox.Text) &&
                !double.TryParse(textBox.Text, out _))
            {
                MessageBox.Show(
                    "Введите корректную цену",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                textBox.Clear();
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле ввода цены.
        /// Для поля минимальной цены проверяет, что введённое значение
        /// не превышает максимально допустимую цену. После проверки
        /// обновляет список отображаемых товаров.
        /// </summary>
        private void PriceBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Name == "PriceFromBox")
            {
                if (double.TryParse(textBox.Text, out double price))
                {
                    if (price > 4925)
                    {
                        MessageBox.Show(
                            "Минимальная цена не может быть больше 4925",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        textBox.Text = "4925";

                        textBox.SelectionStart =
                            textBox.Text.Length;
                    }
                }
            }

            UpdateProducts();
        }
    }
}
