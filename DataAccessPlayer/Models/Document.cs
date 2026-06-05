using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class Document
{
    public long Id { get; set; }

    public long SubjectId { get; set; }

    public long? ChapterId { get; set; }

    public string Title { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long FileSize { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public DateTime? IndexedAt { get; set; }

    public long? UserId { get; set; }

    public long? UploadedBy { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual ICollection<DocumentChunk> DocumentChunks { get; set; } = new List<DocumentChunk>();

    public virtual ICollection<MessageCitation> MessageCitations { get; set; } = new List<MessageCitation>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual User? UploadedByNavigation { get; set; }
}
