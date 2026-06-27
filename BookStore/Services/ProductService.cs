using BookStore.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services;

/// <summary>
/// Сервис для работы с товарами.
/// </summary>
public class ProductService
{
    /// <summary>
    /// Получение списка товаров.
    /// </summary>
    /// <returns>Список товаров.</returns>
    public List<Product> GetProducts()
    {
        try
        {
            using AppDbContext databaseContext = new();

            List<Product> products = databaseContext.Products.ToList();

            return products;
        }
        catch (Exception)
        {
            return new List<Product>();
        }
    }
}
