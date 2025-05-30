using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? Level { get; set; }

    public string? UserName { get; set; }
}
