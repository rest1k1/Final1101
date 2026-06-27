using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookStore.Models;

/// <summary>
/// Модель товара.
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

    [Column("Цена")]
    public double Price { get; set; }

    [Column("Производитель")]
    public string Manufacturer { get; set; } = "";

    [Column("Описание товара")]
    public string Description { get; set; } = "";

}