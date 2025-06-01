using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Data;
using Microsoft.EntityFrameworkCore; // 記得加這個

[Route("api/[controller]")]
[ApiController]
public class FavoriteWordsController : ControllerBase
{
	private readonly EnglishLearningDbContext _context;

	public FavoriteWordsController(EnglishLearningDbContext context)
	{
		_context = context;
	}

	// 新增或更新收藏單字
	[HttpPost]
	public async Task<IActionResult> AddOrUpdateFavoriteWord([FromBody] FavoriteWord favWord)
	{
		// 1. 查詢這個 user 是否已經收藏過這個單字
		var existing = await _context.FavoriteWords
			.FirstOrDefaultAsync(fw => fw.UserId == favWord.UserId && fw.Word == favWord.Word);

		if (existing != null)
		{
			// 2. 已經有，ClickCount +1，更新時間
			existing.ClickCount = (existing.ClickCount) + 1; // 防呆
			existing.FavoritedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return Ok(existing);
		}
		else
		{
			// 3. 沒有，新增一筆，ClickCount=1
			favWord.FavoritedAt = DateTime.Now;
			favWord.ClickCount = 1;
			_context.FavoriteWords.Add(favWord);
			await _context.SaveChangesAsync();
			return Ok(favWord);
		}
	}
}
