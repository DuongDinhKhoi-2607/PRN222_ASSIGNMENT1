using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class EvaluationResult
{
    public long Id { get; set; }

    public long ExperimentRunId { get; set; }

    public long TestQuestionId { get; set; }

    public string? GeneratedAnswer { get; set; }

    public string? RetrievedContexts { get; set; }

    public double? Faithfulness { get; set; }

    public double? AnswerRelevancy { get; set; }

    public double? ContextPrecision { get; set; }

    public double? ContextRecall { get; set; }

    public double? AnswerCorrectness { get; set; }

    public int? LatencyMs { get; set; }

    public virtual ExperimentRun ExperimentRun { get; set; } = null!;

    public virtual TestQuestion TestQuestion { get; set; } = null!;
}
