namespace Autoparts.Domain.Enums;

/// <summary>
/// حالات العمل الممكنة لطلب المقاسات
/// </summary>
public enum JobStatus
{
    /// <summary>قيد الانتظار — لم يبدأ العمل بعد</summary>
    Pending = 0,

    /// <summary>قيد التنفيذ — العمل جارٍ حالياً</summary>
    InProgress = 1,

    /// <summary>جاهز للتسليم — تم الانتهاء وينتظر الاستلام</summary>
    ReadyForDelivery = 2,

    /// <summary>مكتمل — تم التسليم والدفع</summary>
    Completed = 3,

    /// <summary>ملغي</summary>
    Cancelled = 4
}

/// <summary>
/// دوال مساعدة لتحويل Enum إلى نص عربي والعكس
/// </summary>
public static class JobStatusExtensions
{
    public static string ToArabic(this JobStatus status) => status switch
    {
        JobStatus.Pending           => "قيد الانتظار",
        JobStatus.InProgress        => "قيد التنفيذ",
        JobStatus.ReadyForDelivery  => "جاهز للتسليم",
        JobStatus.Completed         => "مكتمل",
        JobStatus.Cancelled         => "ملغي",
        _                           => "غير معروف"
    };

    public static JobStatus FromArabic(string arabic) => arabic switch
    {
        "قيد الانتظار"    => JobStatus.Pending,
        "قيد التنفيذ"     => JobStatus.InProgress,
        "جاهز للتسليم"   => JobStatus.ReadyForDelivery,
        "مكتمل"           => JobStatus.Completed,
        "ملغي"            => JobStatus.Cancelled,
        _                 => JobStatus.Pending
    };
}
