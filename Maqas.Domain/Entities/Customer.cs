namespace Maqas.Domain.Entities;

/// <summary>
/// كيان الزبون (Customer) - يحتوي على البيانات الشخصية للعميل وملاحظاته
/// </summary>
public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<MeasurementSession> MeasurementSessions { get; set; } = new List<MeasurementSession>();
}
