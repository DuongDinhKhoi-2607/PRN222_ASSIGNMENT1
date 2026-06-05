using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class DocumentChunk
{
    public long Id { get; set; }

    public long DocumentId { get; set; }

    public long ChunkingStrategyId { get; set; }

    public int ChunkIndex { get; set; }

    public string Content { get; set; } = null!;

    public int TokenCount { get; set; }

    public int? PageNumber { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChunkEmbedding> ChunkEmbeddings { get; set; } = new List<ChunkEmbedding>();

    public virtual ChunkingStrategy ChunkingStrategy { get; set; } = null!;

    public virtual Document Document { get; set; } = null!;

    public virtual ICollection<MessageCitation> MessageCitations { get; set; } = new List<MessageCitation>();
}
