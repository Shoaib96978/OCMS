namespace OCMS.DTOs.Contact;

public record ContactMessageDto(
    string Name,
    string Email,
    string? Phone,
    string Subject,
    string Message
);
