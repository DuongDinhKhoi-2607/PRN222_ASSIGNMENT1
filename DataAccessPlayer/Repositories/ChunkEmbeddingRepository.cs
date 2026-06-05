using DataAccessPlayer.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class ChunkEmbeddingRepository
    {
        private readonly RagchatbotDbContext _db;
        public ChunkEmbeddingRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<ChunkEmbedding> AddAsync(ChunkEmbedding embedding)
        {
            _db.ChunkEmbeddings.Add(embedding);
            await _db.SaveChangesAsync();
            return embedding;
        }

        public async Task<IEnumerable<ChunkEmbedding>> GetAllWithChunkAsync()
        {
            return await _db.ChunkEmbeddings
                .Include(e => e.Chunk)
                .ThenInclude(c => c.Document)
                .ThenInclude(d => d.Subject)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChunkEmbedding>> GetBySubjectAsync(long subjectId)
        {
            return await _db.ChunkEmbeddings
                .Include(e => e.Chunk)
                .ThenInclude(c => c.Document)
                .ThenInclude(d => d.Subject)
                .Where(e => e.Chunk.Document.SubjectId == subjectId)
                .ToListAsync();
        }
    }
}