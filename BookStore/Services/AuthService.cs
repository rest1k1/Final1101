using BookStore.Data;
using BookStore.Models;

namespace BookStore.Services;

/// <summary>
/// Сервис авторизации.
/// </summary>
public class AuthService
{
    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    public User? Login(
        string login,
        string password)
    {
        using AppDbContext context = new();

        return context.Users.FirstOrDefault(user =>
            user.Login == login &&
            user.Password == password);
    }
}