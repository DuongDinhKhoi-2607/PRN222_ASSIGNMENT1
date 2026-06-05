using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;

namespace BussinessLayer.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatSessionRepository _sessionRepo;
        private readonly ChatMessageRepository _messageRepo;

        public ChatService(ChatSessionRepository sessionRepo, ChatMessageRepository messageRepo)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
        }

        private static ChatSessionDto MapSession(ChatSession s) => new ChatSessionDto
        {
            Id = s.Id,
            UserId = s.UserId,
            SubjectId = s.SubjectId,
            Title = s.Title,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };

        private static ChatMessageDto MapMessage(ChatMessage m) => new ChatMessageDto
        {
            Id = m.Id,
            SessionId = m.SessionId,
            Role = m.Role,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        };

        public async Task<ChatSessionDto> CreateSessionAsync(long userId, long? subjectId = null, string? title = null)
        {
            var s = new ChatSession
            {
                UserId = userId,
                SubjectId = subjectId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var created = await _sessionRepo.AddAsync(s);
            return MapSession(created);
        }

        public async Task<ChatMessageDto> AddMessageAsync(long sessionId, string role, string content)
        {
            var m = new ChatMessage
            {
                SessionId = sessionId,
                Role = role,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _messageRepo.AddAsync(m);
            return MapMessage(created);
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(long sessionId)
        {
            var list = await _messageRepo.GetBySessionAsync(sessionId);
            return list.Select(MapMessage);
        }

        public async Task<IEnumerable<ChatSessionDto>> GetSessionsAsync(long userId)
        {
            var list = await _sessionRepo.GetByUserIdAsync(userId);
            return list.Select(MapSession);
        }

        public async Task<ChatSessionDto?> GetSessionAsync(long sessionId)
        {
            var s = await _sessionRepo.GetByIdAsync(sessionId);
            return s == null ? null : MapSession(s);
        }

        public async Task UpdateSessionTitleAsync(long sessionId, string title)
        {
            await _sessionRepo.UpdateTitleAsync(sessionId, title);
        }
    }
}
