using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class LlmModel
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string? BaseModel { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ExperimentRun> ExperimentRuns { get; set; } = new List<ExperimentRun>();
}
