using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;

namespace BussinessLayer.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentRepository _repo;
        private readonly DocumentChunkRepository _chunkRepo;
        private readonly IFileStorageService _storage;

        public DocumentService(DocumentRepository repo, DocumentChunkRepository chunkRepo, IFileStorageService storage)
        {
            _repo = repo;
            _chunkRepo = chunkRepo;
            _storage = storage;
        }

        private static DocumentDto MapToDto(Document d) => new DocumentDto
        {
            Id = d.Id,
            SubjectId = d.SubjectId,
            ChapterId = d.ChapterId,
            Title = d.Title,
            FileName = d.FileName,
            FileType = d.FileType,
            FileSize = d.FileSize,
            Status = d.Status,
            UploadedAt = d.UploadedAt,
            IndexedAt = d.IndexedAt,
            UserId = d.UserId,
            SubjectName = d.Subject?.Name,
            SubjectCode = d.Subject?.Code,
            UploadedByName = d.UploadedByNavigation?.FullName
        };

        public async Task<IEnumerable<DocumentDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<DocumentDto>> GetBySubjectIdAsync(long subjectId)
        {
            var list = await _repo.GetBySubjectIdAsync(subjectId);
            return list.Select(MapToDto);
        }

        public async Task<DocumentDto?> GetByIdAsync(long id)
        {
            var d = await _repo.GetByIdAsync(id);
            return d == null ? null : MapToDto(d);
        }

        public async Task<DocumentDto?> GetByIdWithSubjectAsync(long id)
        {
            var d = await _repo.GetByIdWithSubjectAsync(id);
            return d == null ? null : MapToDto(d);
        }

        public async Task UpdateAsync(long id, string title, long subjectId)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc != null)
            {
                doc.Title = title;
                doc.SubjectId = subjectId;
                await _repo.UpdateAsync(doc);
            }
        }

        public async Task DeleteAsync(long id)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc != null)
            {
                if (!string.IsNullOrEmpty(doc.FilePath))
                {
                    try
                    {
                        if (System.IO.File.Exists(doc.FilePath))
                            System.IO.File.Delete(doc.FilePath);
                    }
                    catch { /* ignore */ }
                }
                await _repo.DeleteAsync(id);
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetByUserIdAsync(long userId)
        {
            var list = await _repo.GetAllAsync();
            return list.Where(d => d.UserId == userId).Select(MapToDto);
        }

        public async Task<IEnumerable<DocumentChunk>> GetChunksByDocumentIdAsync(long documentId)
        {
            return await _chunkRepo.GetByDocumentIdAsync(documentId);
        }
    }
}
