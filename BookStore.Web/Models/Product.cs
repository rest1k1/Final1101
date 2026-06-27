using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Web.Models;

/// <summary>
/// Представляет информацию о товаре (книге).
/// </summary>
[Table("Products")]
public class Product
{
    [Key]
    public int Id { get; set; }

    [Column("Артикул")]
    public string Article { get; set; } = "";

    [Column("Наименование товара")]
    public string Name { get; set; } = "";

    [Column("Единица измерения")]
    public string Unit { get; set; } = "";

    [Column("Цена")]
    public double Price { get; set; }

    [Column("Автор")]
    public string Author { get; set; } = "";

    [Column("Производитель")]
    public string Manufacturer { get; set; } = "";

    [Column("Категория товара")]
    public string Category { get; set; } = "";

    [Column("Действующая скидка")]
    public int Discount { get; set; }

    [Column("Кол-во на складе")]
    public int StockCount { get; set; }

    [Column("Описание товара")]
    public string Description { get; set; } = "";

    [Column("Фото")]
    public string Photo { get; set; } = "";
}