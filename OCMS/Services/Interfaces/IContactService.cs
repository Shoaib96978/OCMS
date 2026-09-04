using OCMS.DTOs.Contact;
using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface IContactService
    {
        Task<AppResponse> SendMessageAsync(ContactMessageDto dto);
    }
}