using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class User
{
    public int SerialNo { get; set; }

    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? Level { get; set; }

    public string? UserName { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<FavoriteWord> FavoriteWords { get; set; } = new List<FavoriteWord>();

    public virtual ICollection<WordHistory> WordHistories { get; set; } = new List<WordHistory>();
}
