using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class ChatSessionRepository
    {
        private readonly RagchatbotDbContext _db;
        public ChatSessionRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<ChatSession> AddAsync(ChatSession session)
        {
            _db.ChatSessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task<IEnumerable<ChatSession>> GetByUserIdAsync(long userId)
        {
            return await _db.ChatSessions.Where(s => s.UserId == userId).OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt).ToListAsync();
        }

        public async Task<ChatSession?> GetByIdAsync(long id) => await _db.ChatSessions.FindAsync(id);

        public async Task UpdateTitleAsync(long sessionId, string title)
        {
            var session = await _db.ChatSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.Title = title;
                await _db.SaveChangesAsync();
            }
        }
    }
}
