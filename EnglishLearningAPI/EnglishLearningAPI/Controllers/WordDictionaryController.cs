using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Data;

namespace EnglishLearningAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WordDictionaryController : ControllerBase
	{
		private readonly EnglishLearningDbContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public WordDictionaryController(EnglishLearningDbContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
		}

		//  GET: 批次查詢資料庫中已有的單字
		[HttpGet("BulkCheck")]
		public async Task<ActionResult<IEnumerable<WordDictionary>>> BulkCheck([FromQuery(Name = "words")] List<string> words)
		{
			var lowerWords = words.Select(w => w.ToLower()).ToList();
			var found = await _context.WordDictionaries
				.Where(w => lowerWords.Contains(w.Word.ToLower()))
				.ToListAsync();
			return Ok(found);
		}


		//  POST: 批次寫入字典查詢結果（由前端統一送進來）
		[HttpPost("BulkInsert")]
		public async Task<IActionResult> BulkInsert(List<WordDictionary> newWords)
		{
			var existingWords = await _context.WordDictionaries
				.Select(w => w.Word.ToLower())
				.ToListAsync();

			var now = DateTime.UtcNow;
			var toInsert = newWords
				.Where(w => !existingWords.Contains(w.Word.ToLower()))
				.Select(w => new WordDictionary
				{
					Word = w.Word,
					Translation = w.Translation,
					Example = w.Example,
					LastUpdated = now
				})
				.ToList();

			if (toInsert.Any())
			{
				_context.WordDictionaries.AddRange(toInsert);
				await _context.SaveChangesAsync();
			}

			return Ok();
		}
	}
}