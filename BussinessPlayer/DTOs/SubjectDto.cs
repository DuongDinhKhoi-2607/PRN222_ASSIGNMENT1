using System.ComponentModel.DataAnnotations;

namespace BussinessLayer.DTOs
{
    public class SubjectDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        [Display(Name = "Mã môn học")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [Display(Name = "Tên môn học")]
        public string Name { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
