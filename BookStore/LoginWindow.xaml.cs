using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace BookStore
{
    /// <summary>
    /// Окно авторизации.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static User? CurrentUser;

        private readonly AuthService _authService = new();

        public LoginWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Введите логин и пароль.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                User? user = _authService.Login(login, password);

                if (user == null)
                {
                    MessageBox.Show(
                        "Пользователь с указанным логином и паролем не найден.",
                        "Ошибка авторизации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                CurrentUser = user;

                CartStorageService.LoadCart();

                SessionService.SaveSession(user.Login);

                DialogResult = true;

                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Произошла ошибка при авторизации.\n{exception.Message}",
                    "Ошибка авторизации",
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
