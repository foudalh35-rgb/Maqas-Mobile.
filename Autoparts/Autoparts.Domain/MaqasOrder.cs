using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Autoparts.Domain.Common;
using Autoparts.Domain.Enums;

namespace Autoparts.Domain;

/// <summary>
/// كيان طلب المقاسات والفواتير (MaqasOrder Entity)
/// الكيان الرئيسي في طبقة Domain — يمثل طلب خياطة واحد
/// </summary>
public class MaqasOrder : BaseEntity
{
    // ==================== بيانات العميل ====================

    /// <summary>اسم العميل الكامل</summary>
    [Required(ErrorMessage = "اسم العميل مطلوب")]
    [StringLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>رقم الهاتف</summary>
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [StringLength(20, ErrorMessage = "رقم الهاتف لا يتجاوز 20 خانة")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>نوع القماش والخيارات</summary>
    [StringLength(200)]
    public string FabricType { get; set; } = string.Empty;

    // ==================== المقاسات الآلية (سم) ====================

    /// <summary>الطول</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 300, ErrorMessage = "الطول يجب أن يكون بين 0 و 300 سم")]
    public decimal Length { get; set; }

    /// <summary>الكتف</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 100, ErrorMessage = "قياس الكتف يجب أن يكون بين 0 و 100 سم")]
    public decimal Shoulder { get; set; }

    /// <summary>الخصر</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 200, ErrorMessage = "قياس الخصر يجب أن يكون بين 0 و 200 سم")]
    public decimal Waist { get; set; }

    /// <summary>الرقبة</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 80, ErrorMessage = "قياس الرقبة يجب أن يكون بين 0 و 80 سم")]
    public decimal Neck { get; set; }

    /// <summary>الذراع (الواسعة)</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 100, ErrorMessage = "قياس الذراع يجب أن يكون بين 0 و 100 سم")]
    public decimal Arm { get; set; }

    // ==================== الحسابات المالية ====================

    /// <summary>الإجمالي الكامل</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "المبلغ الإجمالي لا يمكن أن يكون سالباً")]
    public decimal TotalPrice { get; set; }

    /// <summary>إعلانات / العربون المدفوع مسبقاً</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "العربون لا يمكن أن يكون سالباً")]
    public decimal AdvancePayment { get; set; }

    /// <summary>المبلغ المدفوع</summary>
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "المدفوع لا يمكن أن يكون سالباً")]
    public decimal Paid { get; set; }

    /// <summary>
    /// المتبقي يُحسب آلياً (Computed Property) — لا يُخزن في قاعدة البيانات
    /// المتبقي = الإجمالي - (العربون + المدفوع)
    /// </summary>
    [NotMapped]
    public decimal Remaining => TotalPrice - (AdvancePayment + Paid);

    /// <summary>هل تم الدفع بالكامل؟</summary>
    [NotMapped]
    public bool IsFullyPaid => Remaining <= 0;

    // ==================== حالة العمل ====================

    /// <summary>حالة العمل بالنص العربي</summary>
    [StringLength(50)]
    public string JobStatus { get; set; } = Enums.JobStatus.InProgress.ToArabic();

    // ==================== Domain Methods ====================

    /// <summary>تحديث حالة الطلب وضبط تاريخ التعديل</summary>
    public void UpdateStatus(JobStatus newStatus)
    {
        JobStatus = newStatus.ToArabic();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>تسجيل دفعة جديدة وتحديث المدفوع</summary>
    public void RegisterPayment(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر");
        Paid += amount;
        UpdatedAt = DateTime.UtcNow;
    }
}
