namespace Maqas.Domain.Entities;

/// <summary>
/// كيان الطلب (Order) - يحفظ تفاصيل الطلب والدفع الخاصة بالعميل والتفصيل
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty; // مثل MQ-8942
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string TailorName { get; set; } = string.Empty;
    public string ItemTitle { get; set; } = string.Empty;
    public string ProductImg { get; set; } = string.Empty;
    public string Fabric { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string ReceiptDate { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string PaidAmount { get; set; } = string.Empty;
    public string Wallet { get; set; } = string.Empty;
    public string RefNo { get; set; } = string.Empty;
    public string Status { get; set; } = "قيد التفصيل";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
