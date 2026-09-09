using System.ComponentModel.DataAnnotations;

namespace Maqas.Domain.Entities;

/// <summary>
/// كيان نوع الملابس (ClothingType) - تصنيف قطعة الملابس (مثل: فستان، قميص، ثوب، بدلة)
/// </summary>
public class ClothingType
{
    [Key]
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;

    public ICollection<MeasurementSession> MeasurementSessions { get; set; } = new List<MeasurementSession>();
}
