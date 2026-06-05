using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;

namespace BussinessLayer.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly SubjectRepository _repo;
        public SubjectService(SubjectRepository repo) { _repo = repo; }

        private static SubjectDto MapToDto(Subject s) => new SubjectDto
        {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            Description = s.Description
        };

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<SubjectDto?> GetByIdAsync(long id)
        {
            var s = await _repo.GetByIdAsync(id);
            return s == null ? null : MapToDto(s);
        }

        public async Task<SubjectDto> CreateAsync(SubjectDto subject)
        {
            var ent = new Subject
            {
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                CreatedAt = System.DateTime.UtcNow
            };
            var created = await _repo.AddAsync(ent);
            return MapToDto(created);
        }

        public async Task UpdateAsync(SubjectDto subject)
        {
            var ent = await _repo.GetByIdAsync(subject.Id);
            if (ent != null)
            {
                ent.Code = subject.Code;
                ent.Name = subject.Name;
                ent.Description = subject.Description;
                await _repo.UpdateAsync(ent);
            }
        }

        public async Task DeleteAsync(long id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, long? ignoreId = null)
        {
            var existing = await _repo.GetByCodeAsync(code);
            if (existing == null) return true;
            if (ignoreId.HasValue && existing.Id == ignoreId.Value) return true;
            return false;
        }
    }
}
