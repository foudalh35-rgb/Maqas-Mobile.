namespace Autoparts.Domain.Common;

/// <summary>
/// الكلاس الأساسي لجميع الكيانات (Base Entity)
/// يحتوي على المعرّف الرئيسي وبيانات التتبع الزمني
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    /// <summary>تاريخ ووقت إنشاء السجل</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>تاريخ ووقت آخر تعديل</summary>
    public DateTime? UpdatedAt { get; set; }
}
