using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class User
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<LecturerUploadPermission> LecturerUploadPermissionGrantedByNavigations { get; set; } = new List<LecturerUploadPermission>();

    public virtual ICollection<LecturerUploadPermission> LecturerUploadPermissionLecturers { get; set; } = new List<LecturerUploadPermission>();
}
