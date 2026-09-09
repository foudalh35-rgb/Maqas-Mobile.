namespace Maqas.Domain.Entities;

/// <summary>
/// كيان المستخدم (User) - يمثل مستخدم النظام المسجل (خياط / زبون)
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
