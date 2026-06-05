using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ChunkingStrategy
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int ChunkSize { get; set; }

    public int ChunkOverlap { get; set; }

    public string? Params { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<DocumentChunk> DocumentChunks { get; set; } = new List<DocumentChunk>();

    public virtual ICollection<ExperimentRun> ExperimentRuns { get; set; } = new List<ExperimentRun>();
}
