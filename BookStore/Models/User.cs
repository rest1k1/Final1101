using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

/// <summary>
/// Модель пользователя.
/// </summary>
[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Column("Роль сотрудника")]
    public string Role { get; set; } = "";

    [Column("ФИО")]
    public string FullName { get; set; } = "";

    [Column("Логин")]
    public string Login { get; set; } = "";

    [Column("Пароль")]
    public string Password { get; set; } = "";
}