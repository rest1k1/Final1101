using BookStore.Web.Data;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Web.Controllers;

/// <summary>
/// Контроллер для отображения товаров и оформления заказа.
/// </summary>
public class ProductsController : Controller
{
    private readonly AppDbContext _databaseContext;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера товаров.
    /// </summary>
    public ProductsController(
        AppDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Отображает список всех товаров.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        List<Product> products =
            await _databaseContext.Products.ToListAsync();

        return View(products);
    }

    /// <summary>
    /// Отображает страницу оформления заказа для выбранного товара.
    /// </summary>
    public async Task<IActionResult> Order(int id)
    {
        Product? product =
            await _databaseContext.Products
                .FirstOrDefaultAsync(
                    databaseProduct =>
                        databaseProduct.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
