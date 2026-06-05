using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BussinessLayer.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly LecturerUploadPermissionRepository _permRepo;

        public PermissionService(LecturerUploadPermissionRepository permRepo)
        {
            _permRepo = permRepo;
        }

        public async Task<IEnumerable<LecturerPermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permRepo.GetAllWithDetailsAsync();
            return permissions.Select(p => new LecturerPermissionDto
            {
                Id = p.Id,
                LecturerId = p.LecturerId,
                LecturerName = p.Lecturer?.FullName ?? "N/A",
                LecturerEmail = p.Lecturer?.Email ?? "N/A",
                SubjectId = p.SubjectId,
                SubjectCode = p.Subject?.Code ?? "N/A",
                SubjectName = p.Subject?.Name ?? "N/A",
                CanUpload = p.CanUpload ?? false,
                GrantedAt = p.GrantedAt,
                GrantedByName = p.GrantedByNavigation?.FullName ?? "N/A"
            });
        }

        public async Task<bool> HasUploadPermissionAsync(long lecturerId, long subjectId)
        {
            return await _permRepo.HasPermissionAsync(lecturerId, subjectId);
        }

        public async Task<IEnumerable<long>> GetAllowedSubjectIdsAsync(long lecturerId)
        {
            return await _permRepo.GetAllowedSubjectIdsAsync(lecturerId);
        }

        public async Task GrantPermissionAsync(GrantPermissionDto dto, long grantedByUserId)
        {
            // Check if permission already exists
            var existing = await _permRepo.GetByLecturerAndSubjectAsync(dto.LecturerId, dto.SubjectId);
            if (existing != null)
            {
                // Update existing permission
                existing.CanUpload = dto.CanUpload;
                existing.GrantedBy = grantedByUserId;
                existing.GrantedAt = DateTime.Now;
                // Save is handled via tracked entity
                await _permRepo.DeleteAsync(existing.Id);
                await _permRepo.AddAsync(new LecturerUploadPermission
                {
                    LecturerId = dto.LecturerId,
                    SubjectId = dto.SubjectId,
                    CanUpload = dto.CanUpload,
                    GrantedBy = grantedByUserId,
                    GrantedAt = DateTime.Now
                });
                return;
            }

            var permission = new LecturerUploadPermission
            {
                LecturerId = dto.LecturerId,
                SubjectId = dto.SubjectId,
                CanUpload = dto.CanUpload,
                GrantedBy = grantedByUserId,
                GrantedAt = DateTime.Now
            };

            await _permRepo.AddAsync(permission);
        }

        public async Task RevokePermissionAsync(long permissionId)
        {
            await _permRepo.DeleteAsync(permissionId);
        }

        public async Task<bool> PermissionExistsAsync(long lecturerId, long subjectId)
        {
            return await _permRepo.ExistsAsync(lecturerId, subjectId);
        }

        public async Task<bool> IsSubjectAssignedToAnotherLecturerAsync(long subjectId, long currentLecturerId)
        {
            return await _permRepo.IsSubjectAssignedToAnotherLecturerAsync(subjectId, currentLecturerId);
        }

        public async Task<string?> GetAssignedLecturerNameAsync(long subjectId)
        {
            return await _permRepo.GetAssignedLecturerNameAsync(subjectId);
        }
    }
}
