using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class LecturerUploadPermission
{
    public long Id { get; set; }

    public long LecturerId { get; set; }

    public long SubjectId { get; set; }

    public bool? CanUpload { get; set; }

    public long GrantedBy { get; set; }

    public DateTime? GrantedAt { get; set; }

    public virtual User GrantedByNavigation { get; set; } = null!;

    public virtual User Lecturer { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
