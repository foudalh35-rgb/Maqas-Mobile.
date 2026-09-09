namespace Maqas.Domain.Entities;

/// <summary>
/// كيان جلسة القياس (MeasurementSession) - يمثل عملية أخذ القياس لطلب تفصيل أو قطعة جاهزة
/// </summary>
public class MeasurementSession
{
    public int SessionId { get; set; }
    public DateTime DateMeasured { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "مقاسات جديدة";

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int TypeId { get; set; }
    public ClothingType? ClothingType { get; set; }

    public ICollection<MeasurementDetail> Details { get; set; } = new List<MeasurementDetail>();
}
