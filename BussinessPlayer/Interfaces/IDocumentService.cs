using BussinessLayer.DTOs;
using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>> GetAllAsync();
        Task<IEnumerable<DocumentDto>> GetBySubjectIdAsync(long subjectId);
        Task<DocumentDto?> GetByIdAsync(long id);
        Task<DocumentDto?> GetByIdWithSubjectAsync(long id);
        Task UpdateAsync(long id, string title, long subjectId);
        Task DeleteAsync(long id);
        Task<IEnumerable<DocumentDto>> GetByUserIdAsync(long userId);
        Task<IEnumerable<DocumentChunk>> GetChunksByDocumentIdAsync(long documentId);
    }
}
