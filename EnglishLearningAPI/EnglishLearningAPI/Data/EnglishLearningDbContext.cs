using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Data
{
	public class EnglishLearningDbContext : DbContext
	{
		public EnglishLearningDbContext(DbContextOptions<EnglishLearningDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }


		protected EnglishLearningDbContext()
		{
		}
	}
}
