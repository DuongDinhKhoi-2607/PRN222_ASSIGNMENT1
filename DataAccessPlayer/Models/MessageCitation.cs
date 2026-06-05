using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class MessageCitation
{
    public long Id { get; set; }

    public long MessageId { get; set; }

    public long ChunkId { get; set; }

    public long DocumentId { get; set; }

    public double? RelevanceScore { get; set; }

    public string? Snippet { get; set; }

    public virtual DocumentChunk Chunk { get; set; } = null!;

    public virtual Document Document { get; set; } = null!;

    public virtual ChatMessage Message { get; set; } = null!;
}
