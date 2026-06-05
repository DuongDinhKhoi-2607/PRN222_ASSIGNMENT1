using System;
using System.ComponentModel.DataAnnotations;

namespace BussinessLayer.DTOs
{
    public class LecturerPermissionDto
    {
        public long Id { get; set; }

        public long LecturerId { get; set; }

        [Display(Name = "Giảng viên")]
        public string LecturerName { get; set; } = null!;

        [Display(Name = "Email")]
        public string LecturerEmail { get; set; } = null!;

        public long SubjectId { get; set; }

        [Display(Name = "Mã môn")]
        public string SubjectCode { get; set; } = null!;

        [Display(Name = "Môn học")]
        public string SubjectName { get; set; } = null!;

        [Display(Name = "Quyền Upload")]
        public bool CanUpload { get; set; }

        [Display(Name = "Ngày cấp")]
        public DateTime? GrantedAt { get; set; }

        [Display(Name = "Cấp bởi")]
        public string GrantedByName { get; set; } = null!;
    }

    public class GrantPermissionDto
    {
        [Required(ErrorMessage = "Vui lòng chọn giảng viên")]
        [Display(Name = "Giảng viên")]
        public long LecturerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public long SubjectId { get; set; }

        [Display(Name = "Cho phép Upload")]
        public bool CanUpload { get; set; } = true;
    }
}
