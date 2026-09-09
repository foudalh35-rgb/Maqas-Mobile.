namespace Maqas.Domain.Entities;

/// <summary>
/// كيان تفاصيل القياس (MeasurementDetail) - يمثل قيمة قياس محددة (مثل: الصدر: 95 سم)
/// </summary>
public class MeasurementDetail
{
    public int DetailId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public float Value { get; set; }
    public string TypeUnit { get; set; } = "سم";

    public int SessionId { get; set; }
    public MeasurementSession? Session { get; set; }
}
