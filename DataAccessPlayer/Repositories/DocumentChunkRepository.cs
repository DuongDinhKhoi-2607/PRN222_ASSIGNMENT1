using DataAccessPlayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DocumentChunkRepository
    {
        private readonly RagchatbotDbContext _db;
        public DocumentChunkRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<DocumentChunk> AddAsync(DocumentChunk chunk)
        {
            if (chunk == null) throw new ArgumentNullException(nameof(chunk));

            // Ensure the referenced Document exists or is tracked by the same context.
            if (chunk.Document != null)
            {
                if (chunk.Document.Id == 0)
                {
                    // New document: add it first so it gets an Id
                    _db.Documents.Add(chunk.Document);
                    await _db.SaveChangesAsync();
                    chunk.DocumentId = chunk.Document.Id;
                }
                else
                {
                    // Existing document: make sure it exists in DB and attach to context so EF won't try to insert it
                    var exists = await _db.Documents.AnyAsync(d => d.Id == chunk.Document.Id);
                    if (!exists)
                        throw new InvalidOperationException($"Document with Id {chunk.Document.Id} does not exist.");

                    _db.Documents.Attach(chunk.Document);
                    chunk.DocumentId = chunk.Document.Id;
                }
            }
            else if (chunk.DocumentId != 0)
            {
                var exists = await _db.Documents.AnyAsync(d => d.Id == chunk.DocumentId);
                if (!exists)
                    throw new InvalidOperationException($"Document with Id {chunk.DocumentId} does not exist.");
            }
            else
            {
                throw new InvalidOperationException("DocumentChunk must have either Document or DocumentId set.");
            }

            _db.DocumentChunks.Add(chunk);
            await _db.SaveChangesAsync();
            return chunk;
        }

        public async Task<IEnumerable<DocumentChunk>> GetByDocumentIdAsync(long documentId)
        {
            return await _db.DocumentChunks.Where(c => c.DocumentId == documentId).ToListAsync();
        }
    }
}