using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class Experiment
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ExperimentRun> ExperimentRuns { get; set; } = new List<ExperimentRun>();
}
