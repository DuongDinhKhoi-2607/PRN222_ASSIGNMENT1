using System;
using System.ComponentModel.DataAnnotations;

namespace BussinessLayer.DTOs
{
    public class ChapterDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public long SubjectId { get; set; }

        [Required(ErrorMessage = "Tiêu đề chapter là bắt buộc")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Display(Name = "Thứ tự")]
        public int OrderIndex { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }
    }
}
