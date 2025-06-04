using System.ComponentModel.DataAnnotations.Schema;

[Table("WordDictionary")]
public class WordDictionary
{
	public int Id { get; set; }
	public string Word { get; set; } = null!;
	public string Translation { get; set; } = null!;
	public string Example { get; set; } = null!;
	public DateTime LastUpdated { get; set; }
}
