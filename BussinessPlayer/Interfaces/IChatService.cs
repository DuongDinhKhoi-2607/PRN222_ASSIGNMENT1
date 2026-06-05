using BussinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IChatService
    {
        Task<ChatSessionDto> CreateSessionAsync(long userId, long? subjectId = null, string? title = null);
        Task<ChatMessageDto> AddMessageAsync(long sessionId, string role, string content);
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(long sessionId);
        Task<IEnumerable<ChatSessionDto>> GetSessionsAsync(long userId);
        Task<ChatSessionDto?> GetSessionAsync(long sessionId);
        Task UpdateSessionTitleAsync(long sessionId, string title);
    }
}
