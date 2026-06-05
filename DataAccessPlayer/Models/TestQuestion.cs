using System;
using System.Collections.Generic;

namespace DataAccessPlayer.Models;

public partial class TestQuestion
{
    public long Id { get; set; }

    public long SubjectId { get; set; }

    public string Question { get; set; } = null!;

    public string GroundTruth { get; set; } = null!;

    public string? ReferenceContext { get; set; }

    public string? Difficulty { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();

    public virtual Subject Subject { get; set; } = null!;
}
