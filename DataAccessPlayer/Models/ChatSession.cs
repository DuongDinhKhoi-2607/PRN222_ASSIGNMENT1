using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ChatSession
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? SubjectId { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual Subject? Subject { get; set; }

    public virtual User User { get; set; } = null!;
}
