using BussinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<LecturerPermissionDto>> GetAllPermissionsAsync();
        Task<bool> HasUploadPermissionAsync(long lecturerId, long subjectId);
        Task<IEnumerable<long>> GetAllowedSubjectIdsAsync(long lecturerId);
        Task GrantPermissionAsync(GrantPermissionDto dto, long grantedByUserId);
        Task RevokePermissionAsync(long permissionId);
        Task<bool> PermissionExistsAsync(long lecturerId, long subjectId);
        Task<bool> IsSubjectAssignedToAnotherLecturerAsync(long subjectId, long currentLecturerId);
        Task<string?> GetAssignedLecturerNameAsync(long subjectId);
    }
}
