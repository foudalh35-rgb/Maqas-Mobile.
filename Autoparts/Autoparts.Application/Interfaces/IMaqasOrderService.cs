using Autoparts.Application.DTOs;

namespace Autoparts.Application.Interfaces;

/// <summary>
/// واجهة خدمة المقاسات — تُعرّف عمليات الأعمال بدون تفاصيل التنفيذ
/// </summary>
public interface IMaqasOrderService
{
    /// <summary>جلب جميع طلبات المقاسات</summary>
    Task<IEnumerable<MaqasOrderDto>> GetAllAsync();

    /// <summary>جلب طلب مقاسات بواسطة المعرّف</summary>
    Task<MaqasOrderDto?> GetByIdAsync(int id);

    /// <summary>إنشاء طلب مقاسات جديد</summary>
    Task<MaqasOrderDto> CreateAsync(CreateMaqasOrderDto dto);

    /// <summary>تحديث حالة العمل لطلب موجود</summary>
    Task<bool> UpdateStatusAsync(int id, string newStatus);

    /// <summary>حذف طلب مقاسات</summary>
    Task<bool> DeleteAsync(int id);
}
