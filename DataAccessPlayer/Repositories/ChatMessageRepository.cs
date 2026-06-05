using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class ChatMessageRepository
    {
        private readonly RagchatbotDbContext _db;
        public ChatMessageRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<ChatMessage> AddAsync(ChatMessage message)
        {
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<ChatMessage>> GetBySessionAsync(long sessionId)
        {
            return await _db.ChatMessages.Where(m => m.SessionId == sessionId).ToListAsync();
        }
    }
}
