using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Models;

public partial class Conversation
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TopicId { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

	public virtual Topic Topic { get; set; } = null!;
}
