using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class WordHistory
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Word { get; set; } = null!;

    public int ClickCount { get; set; }

    public DateTime FirstViewedAt { get; set; }

    public DateTime LastViewedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
