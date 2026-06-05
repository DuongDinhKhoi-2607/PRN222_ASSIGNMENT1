using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ChunkEmbedding
{
    public long Id { get; set; }

    public long ChunkId { get; set; }

    public long EmbeddingModelId { get; set; }

    public string Vector { get; set; } = null!;

    public int Dimension { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual DocumentChunk Chunk { get; set; } = null!;

    public virtual EmbeddingModel EmbeddingModel { get; set; } = null!;
}
