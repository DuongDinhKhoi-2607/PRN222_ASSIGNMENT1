using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class ExperimentRunMetric
{
    public long Id { get; set; }

    public long ExperimentRunId { get; set; }

    public double? AvgFaithfulness { get; set; }

    public double? AvgAnswerRelevancy { get; set; }

    public double? AvgContextPrecision { get; set; }

    public double? AvgContextRecall { get; set; }

    public double? AvgAnswerCorrectness { get; set; }

    public double? AvgLatencyMs { get; set; }

    public int? TotalQuestions { get; set; }

    public virtual ExperimentRun ExperimentRun { get; set; } = null!;
}
