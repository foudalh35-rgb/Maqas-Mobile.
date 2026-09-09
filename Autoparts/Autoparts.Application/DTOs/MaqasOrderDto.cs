using Autoparts.Domain;

namespace Autoparts.Application.DTOs;

/// <summary>
/// DTO لإنشاء طلب مقاسات جديد (يُرسل من الواجهة إلى الـ API)
/// </summary>
public class CreateMaqasOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FabricType { get; set; } = string.Empty;

    // المقاسات
    public decimal Length { get; set; }
    public decimal Shoulder { get; set; }
    public decimal Waist { get; set; }
    public decimal Neck { get; set; }
    public decimal Arm { get; set; }

    // المالية
    public decimal TotalPrice { get; set; }
    public decimal AdvancePayment { get; set; }
    public decimal Paid { get; set; }

    // الحالة
    public string JobStatus { get; set; } = "قيد التنفيذ";
}

/// <summary>
/// DTO لإرجاع بيانات الطلب (يُرسل من الـ API إلى الواجهة)
/// </summary>
public class MaqasOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FabricType { get; set; } = string.Empty;

    // المقاسات
    public decimal Length { get; set; }
    public decimal Shoulder { get; set; }
    public decimal Waist { get; set; }
    public decimal Neck { get; set; }
    public decimal Arm { get; set; }

    // المالية
    public decimal TotalPrice { get; set; }
    public decimal AdvancePayment { get; set; }
    public decimal Paid { get; set; }
    public decimal Remaining => TotalPrice - (AdvancePayment + Paid); // آلي

    // الحالة والتاريخ
    public string JobStatus { get; set; } = string.Empty;

    // تحويل من الكيان إلى الـ DTO
    public static MaqasOrderDto FromEntity(MaqasOrder order) => new MaqasOrderDto
    {
        Id = order.Id,
        CustomerName = order.CustomerName,
        Phone = order.Phone,
        FabricType = order.FabricType,
        Length = order.Length,
        Shoulder = order.Shoulder,
        Waist = order.Waist,
        Neck = order.Neck,
        Arm = order.Arm,
        TotalPrice = order.TotalPrice,
        AdvancePayment = order.AdvancePayment,
        Paid = order.Paid,
        JobStatus = order.JobStatus
    };
}
