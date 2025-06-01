using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class Topic
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
