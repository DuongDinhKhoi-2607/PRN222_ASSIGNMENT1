using DataAccessPlayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DocumentRepository
    {
        private readonly RagchatbotDbContext _db;
        public DocumentRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<Document> AddAsync(Document doc)
        {
            _db.Documents.Add(doc);
            await _db.SaveChangesAsync();
            return doc;
        }

        public async Task<Document> GetByIdAsync(long id) => await _db.Documents.FindAsync(id);

        public async Task<Document?> GetByIdWithSubjectAsync(long id)
            => await _db.Documents.Include(d => d.Subject).Include(d => d.UploadedByNavigation).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<IEnumerable<Document>> GetAllAsync() => await _db.Documents.Include(d => d.Subject).Include(d => d.UploadedByNavigation).ToListAsync();

        public async Task<IEnumerable<Document>> GetBySubjectIdAsync(long subjectId)
            => await _db.Documents.Where(d => d.SubjectId == subjectId).ToListAsync();

        public async Task UpdateAsync(Document doc)
        {
            _db.Documents.Update(doc);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var doc = await _db.Documents.FindAsync(id);
            if (doc != null)
            {
                // Delete chunk embeddings
                var embeddings = _db.ChunkEmbeddings.Where(e => e.Chunk.DocumentId == id);
                _db.ChunkEmbeddings.RemoveRange(embeddings);

                // Delete message citations
                var citations = _db.MessageCitations.Where(c => c.DocumentId == id);
                _db.MessageCitations.RemoveRange(citations);

                // Delete document chunks
                var chunks = _db.DocumentChunks.Where(c => c.DocumentId == id);
                _db.DocumentChunks.RemoveRange(chunks);

                // Delete document itself
                _db.Documents.Remove(doc);

                await _db.SaveChangesAsync();
            }
        }
    }
}