using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ExperimentRun
{
    public long Id { get; set; }

    public long ExperimentId { get; set; }

    public long? EmbeddingModelId { get; set; }

    public long? ChunkingStrategyId { get; set; }

    public long? LlmModelId { get; set; }

    public string? RunName { get; set; }

    public string? Params { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ChunkingStrategy? ChunkingStrategy { get; set; }

    public virtual EmbeddingModel? EmbeddingModel { get; set; }

    public virtual ICollection<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();

    public virtual Experiment Experiment { get; set; } = null!;

    public virtual ExperimentRunMetric? ExperimentRunMetric { get; set; }

    public virtual LlmModel? LlmModel { get; set; }
}
