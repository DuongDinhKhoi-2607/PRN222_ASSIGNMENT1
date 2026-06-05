using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class Chapter
{
    public long Id { get; set; }

    public long SubjectId { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Subject Subject { get; set; } = null!;
}
