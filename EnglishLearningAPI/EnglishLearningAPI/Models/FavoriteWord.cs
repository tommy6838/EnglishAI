using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class FavoriteWord
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Word { get; set; } = null!;

    public DateTime FavoritedAt { get; set; }

    public int ClickCount { get; set; }
}
