using EnglishLearningAPI.Data;
using EnglishLearningAPI.DTOs;
using EnglishLearningAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly EnglishLearningDbContext _context;

        public ConversationsController(EnglishLearningDbContext context)
        {
            _context = context;
        }

		// 新增對話紀錄 API
		[HttpPost]
		public async Task<IActionResult> AddConversation([FromBody] ConversationCreateDto dto)
		{
			var conv = new Conversation
			{
				UserId = dto.UserId,
				TopicId = dto.TopicId,
				Question = dto.Question,
				Answer = dto.Answer,
				CreatedAt = DateTime.Now
			};

			_context.Conversations.Add(conv);
			await _context.SaveChangesAsync();
			return Ok(conv);
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations(string userId)
		{
			var convs = await _context.Conversations
				.Where(c => c.UserId == userId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();

			return Ok(convs);
		}

	}
}
