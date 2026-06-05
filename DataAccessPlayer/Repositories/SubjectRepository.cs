using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class SubjectRepository
    {
        private readonly RagchatbotDbContext _db;
        public SubjectRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<Subject> AddAsync(Subject subject)
        {
            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();
            return subject;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync() => await _db.Subjects.ToListAsync();

        public async Task<Subject?> GetByIdAsync(long id) => await _db.Subjects.FindAsync(id);

        public async Task<Subject?> GetByCodeAsync(string code) => await _db.Subjects.FirstOrDefaultAsync(s => s.Code == code);

        public async Task UpdateAsync(Subject subject)
        {
            _db.Subjects.Update(subject);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject != null)
            {
                // Cascade Delete Documents and their related data
                var documents = await _db.Documents.Where(d => d.SubjectId == id).ToListAsync();
                foreach (var doc in documents)
                {
                    if (!string.IsNullOrEmpty(doc.FilePath))
                    {
                        try { if (System.IO.File.Exists(doc.FilePath)) System.IO.File.Delete(doc.FilePath); }
                        catch { /* ignore */ }
                    }
                    var embeddings = _db.ChunkEmbeddings.Where(e => e.Chunk.DocumentId == doc.Id);
                    _db.ChunkEmbeddings.RemoveRange(embeddings);
                    var docCitations = _db.MessageCitations.Where(c => c.DocumentId == doc.Id);
                    _db.MessageCitations.RemoveRange(docCitations);
                    var chunks = _db.DocumentChunks.Where(c => c.DocumentId == doc.Id);
                    _db.DocumentChunks.RemoveRange(chunks);
                }
                _db.Documents.RemoveRange(documents);

                // Cascade Delete ChatSessions and their related messages
                var sessions = await _db.ChatSessions.Where(s => s.SubjectId == id).ToListAsync();
                foreach (var session in sessions)
                {
                    var msgs = _db.ChatMessages.Where(m => m.SessionId == session.Id);
                    var msgIds = msgs.Select(m => m.Id);
                    var msgCitations = _db.MessageCitations.Where(c => msgIds.Contains(c.MessageId));
                    _db.MessageCitations.RemoveRange(msgCitations);
                    _db.ChatMessages.RemoveRange(msgs);
                }
                _db.ChatSessions.RemoveRange(sessions);

                // Delete Chapters
                var chapters = _db.Chapters.Where(c => c.SubjectId == id);
                _db.Chapters.RemoveRange(chapters);

                // Delete TestQuestions
                var tqs = _db.TestQuestions.Where(t => t.SubjectId == id);
                var tqIds = tqs.Select(t => t.Id);
                var evalResults = _db.EvaluationResults.Where(e => tqIds.Contains(e.TestQuestionId));
                _db.EvaluationResults.RemoveRange(evalResults);
                _db.TestQuestions.RemoveRange(tqs);

                // Delete Permissions
                var perms = _db.LecturerUploadPermissions.Where(p => p.SubjectId == id);
                _db.LecturerUploadPermissions.RemoveRange(perms);

                // Delete Subject
                _db.Subjects.Remove(subject);
                
                await _db.SaveChangesAsync();
            }
        }
    }
}
