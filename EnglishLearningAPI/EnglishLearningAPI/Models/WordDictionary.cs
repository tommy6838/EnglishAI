using System;
using System.Collections.Generic;

namespace EnglishLearningAPI.Models;

public partial class WordDictionary
{
    public int Id { get; set; }

    public string Word { get; set; } = null!;

    public string Translation { get; set; } = null!;

    public string Example { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public string? Definition { get; set; }

    public string? PartOfSpeech { get; set; }

    public string? Phonetic { get; set; }
}
