using BussinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(long id);
        Task<SubjectDto> CreateAsync(SubjectDto subject);
        Task UpdateAsync(SubjectDto subject);
        Task DeleteAsync(long id);
        Task<bool> IsCodeUniqueAsync(string code, long? ignoreId = null);
    }
}
