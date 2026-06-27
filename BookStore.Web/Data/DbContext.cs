using BookStore.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Web.Data;

/// <summary>
/// Контекст базы данных веб-приложения BookStore.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр контекста базы данных.
    /// </summary>
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Таблица товаров.
    /// </summary>
    public DbSet<Product> Products
    {
        get;
        set;
    } = null!;
}