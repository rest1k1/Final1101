using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Модель заказа.
/// </summary>
public class Order
{
    public int Id { get; set; }

    [Column("Номер заказа")]
    public int OrderNumber { get; set; }

    [Column("Артикул заказа")]
    public string? Article { get; set; }

    [Column("Дата заказа")]
    public string? OrderDate { get; set; }

    [Column("Дата доставки")]
    public string? DeliveryDate { get; set; }

    [Column("Статус заказа")]
    public string? Status { get; set; }

    [Column("ФИО авторизованного клиента")]
    public string? ClientName { get; set; }

    [Column("Код для получения")]
    public int? CodeForReceive { get; set; }
}