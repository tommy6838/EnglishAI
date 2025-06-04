using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearningAPI.Models;
using System.Text.RegularExpressions;
using EnglishLearningAPI.Data;

namespace EnglishLearningAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WordCacheController : ControllerBase
	{
		private readonly EnglishLearningDbContext _context;

		public WordCacheController(EnglishLearningDbContext context)
		{
			_context = context;
		}

		[HttpGet("PreloadWords/{userId}")]
		public async Task<ActionResult<IEnumerable<string>>> GetPreloadWords(string userId)
		{
			// 撈取 WordHistory 中的單字
			var historyWords = await _context.WordHistories
				.Where(w => w.UserId == userId)
				.Select(w => w.Word)
				.ToListAsync();

			// 撈取 FavoriteWord 中的單字
			var favoriteWords = await _context.FavoriteWords
				.Where(f => f.UserId == userId)
				.Select(f => f.Word)
				.ToListAsync();

			// 撈取 Conversations 中的回答內容，並斷詞
			var answerTexts = await _context.Conversations
				.Where(c => c.UserId == userId)
				.Select(c => c.Answer)
				.ToListAsync();

			var answerWords = new HashSet<string>();
			foreach (var answer in answerTexts)
			{
				if (!string.IsNullOrWhiteSpace(answer))
				{
					var matches = Regex.Matches(answer, @"\b[\w'-]{3,}\b");
					foreach (Match match in matches)
					{
						answerWords.Add(match.Value.ToLower());
					}
				}
			}

			// 合併 + 去重
			var combined = historyWords
				.Concat(favoriteWords)
				.Concat(answerWords)
				.Where(w => !string.IsNullOrWhiteSpace(w))
				.Select(w => w.Trim().ToLower())
				.Distinct()
				.OrderBy(w => w)
				.ToList();

			return Ok(combined);
		}
	}
}