using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ChatMessage
{
    public long Id { get; set; }

    public long SessionId { get; set; }

    public long? LlmModelId { get; set; }

    public string Role { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? LatencyMs { get; set; }

    public int? TokenUsage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual LlmModel? LlmModel { get; set; }

    public virtual ICollection<MessageCitation> MessageCitations { get; set; } = new List<MessageCitation>();

    public virtual ChatSession Session { get; set; } = null!;
}
