namespace Maqas.Application.DTOs;

// --- DTOs الخاصة بالمستخدمين والتوثيق ---
public record RegisterDto(string Name, string Email, string Password);
public record LoginDto(string Email, string Password, string Role = "Customer");
public record UserResponseDto(int UserId, string Name, string Email);
public record LoginResponseDto(bool Success, string Message, string Role, UserResponseDto? User = null);

// --- DTOs الخاصة بالزبائن ---
public record CreateCustomerDto(string FullName, string Phone, string Notes, int UserId);
public record CustomerResponseDto(int CustomerId, string FullName, string Phone, string Notes, int UserId);
public record UpdateCustomerDto(string FullName, string Phone, string Notes, int UserId);

// --- DTOs الخاصة بأنواع الملابس ---
public record CreateClothingTypeDto(string TypeName);
public record ClothingTypeResponseDto(int TypeId, string TypeName);
public record UpdateClothingTypeDto(string TypeName);

// --- DTOs الخاصة بالقياسات ---
public record MeasurementDetailDto(string AttributeName, float Value, string TypeUnit);

public record CreateMeasurementSessionDto(
    int CustomerId,
    int TypeId,
    string Notes,
    List<MeasurementDetailDto> Details
);

public record MeasurementSessionResponseDto(
    int SessionId,
    int CustomerId,
    string CustomerName,
    int TypeId,
    string ClothingTypeName,
    DateTime DateMeasured,
    string Notes,
    string Status,
    List<MeasurementDetailDto> Details
);

public record UpdateStatusDto(string Status);
public record UpdateUserDto(string Name, string Email, string Password);
public record UpdateMeasurementSessionDto(int CustomerId, int TypeId, string Notes, List<MeasurementDetailDto> Details, string Status);

// --- DTOs الخاصة بالرسائل والدردشة ---
public record SendMessageDto(int SenderId, string SenderRole, int ReceiverId, int? SessionId, string MessageText);
public record ChatMessageResponseDto(int MessageId, int SenderId, string SenderRole, int ReceiverId, int? SessionId, string MessageText, DateTime SentAt, bool IsRead);
