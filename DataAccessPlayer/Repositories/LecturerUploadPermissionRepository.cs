using DataAccessPlayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class LecturerUploadPermissionRepository
    {
        private readonly RagchatbotDbContext _db;

        public LecturerUploadPermissionRepository(RagchatbotDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LecturerUploadPermission>> GetAllWithDetailsAsync()
        {
            return await _db.LecturerUploadPermissions
                .Include(p => p.Lecturer)
                .Include(p => p.Subject)
                .Include(p => p.GrantedByNavigation)
                .OrderByDescending(p => p.GrantedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LecturerUploadPermission>> GetByLecturerIdAsync(long lecturerId)
        {
            return await _db.LecturerUploadPermissions
                .Include(p => p.Subject)
                .Where(p => p.LecturerId == lecturerId && p.CanUpload == true)
                .ToListAsync();
        }

        public async Task<LecturerUploadPermission?> GetByLecturerAndSubjectAsync(long lecturerId, long subjectId)
        {
            return await _db.LecturerUploadPermissions
                .FirstOrDefaultAsync(p => p.LecturerId == lecturerId && p.SubjectId == subjectId);
        }

        public async Task<bool> HasPermissionAsync(long lecturerId, long subjectId)
        {
            return await _db.LecturerUploadPermissions
                .AnyAsync(p => p.LecturerId == lecturerId && p.SubjectId == subjectId && p.CanUpload == true);
        }

        public async Task<IEnumerable<long>> GetAllowedSubjectIdsAsync(long lecturerId)
        {
            return await _db.LecturerUploadPermissions
                .Where(p => p.LecturerId == lecturerId && p.CanUpload == true)
                .Select(p => p.SubjectId)
                .ToListAsync();
        }

        public async Task<LecturerUploadPermission> AddAsync(LecturerUploadPermission permission)
        {
            _db.LecturerUploadPermissions.Add(permission);
            await _db.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> ExistsAsync(long lecturerId, long subjectId)
        {
            return await _db.LecturerUploadPermissions
                .AnyAsync(p => p.LecturerId == lecturerId && p.SubjectId == subjectId);
        }

        public async Task<bool> IsSubjectAssignedToAnotherLecturerAsync(long subjectId, long currentLecturerId)
        {
            return await _db.LecturerUploadPermissions
                .AnyAsync(p => p.SubjectId == subjectId && p.LecturerId != currentLecturerId && p.CanUpload == true);
        }

        public async Task<string?> GetAssignedLecturerNameAsync(long subjectId)
        {
            var perm = await _db.LecturerUploadPermissions
                .Include(p => p.Lecturer)
                .FirstOrDefaultAsync(p => p.SubjectId == subjectId && p.CanUpload == true);
            return perm?.Lecturer?.FullName;
        }

        public async Task<LecturerUploadPermission?> GetByIdAsync(long id)
        {
            return await _db.LecturerUploadPermissions.FindAsync(id);
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _db.LecturerUploadPermissions.FindAsync(id);
            if (entity != null)
            {
                _db.LecturerUploadPermissions.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
