using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class EmbeddingModel
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public int Dimension { get; set; }

    public bool? IsFree { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ChunkEmbedding> ChunkEmbeddings { get; set; } = new List<ChunkEmbedding>();

    public virtual ICollection<ExperimentRun> ExperimentRuns { get; set; } = new List<ExperimentRun>();
}
