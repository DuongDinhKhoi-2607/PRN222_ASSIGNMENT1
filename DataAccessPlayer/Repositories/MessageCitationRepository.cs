using DataAccessPlayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class MessageCitationRepository
    {
        private readonly RagchatbotDbContext _db;
        public MessageCitationRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<MessageCitation> AddAsync(MessageCitation citation)
        {
            _db.MessageCitations.Add(citation);
            await _db.SaveChangesAsync();
            return citation;
        }
    }
}
