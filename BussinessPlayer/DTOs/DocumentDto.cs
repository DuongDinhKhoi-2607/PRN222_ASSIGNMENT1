using System;
using System.ComponentModel.DataAnnotations;

namespace BussinessLayer.DTOs
{
    public class DocumentDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public long SubjectId { get; set; }

        [Display(Name = "Chapter")]
        public long? ChapterId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Display(Name = "Tên file")]
        public string FileName { get; set; } = null!;

        [Display(Name = "Loại file")]
        public string FileType { get; set; } = null!;

        [Display(Name = "Kích thước")]
        public long FileSize { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = null!;

        [Display(Name = "Ngày upload")]
        public DateTime? UploadedAt { get; set; }

        [Display(Name = "Ngày index")]
        public DateTime? IndexedAt { get; set; }

        public long? UserId { get; set; }

        // Navigation display
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? UploadedByName { get; set; }
    }
}
