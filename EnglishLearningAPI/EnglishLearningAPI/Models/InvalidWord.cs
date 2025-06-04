using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearningAPI.Models
{
	public class InvalidWord
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Word { get; set; }

		public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
	}
}
