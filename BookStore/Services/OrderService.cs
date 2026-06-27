using BookStore.Data;
using BookStore.Models;

namespace BookStore.Services;

/// <summary>
/// Сервис для работы с заказами.
/// </summary>
public class OrderService

{
    /// <summary>
    /// Получение следующего номера заказа.
    /// </summary>
    /// <returns>Номер заказа.</returns>
    public int GetNextOrderNumber()
    {
        using var context = new AppDbContext();

        return context.Orders.Any()
            ? context.Orders.Max(o => o.OrderNumber) + 1
            : 1;
    }
}
