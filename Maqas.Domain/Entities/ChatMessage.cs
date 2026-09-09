namespace Maqas.Domain.Entities;

/// <summary>
/// كيان الرسائل (ChatMessage) - يمثل المحادثات والرسائل المتبادلة بين العميل والخياط
/// </summary>
public class ChatMessage
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderRole { get; set; } = "Customer";
    public int ReceiverId { get; set; }
    public int? SessionId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}
