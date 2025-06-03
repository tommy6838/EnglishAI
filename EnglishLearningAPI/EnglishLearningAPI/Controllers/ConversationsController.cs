using EnglishLearningAPI.Data;
using EnglishLearningAPI.DTOs;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options; // 用來注入設定
using OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConversationsController : ControllerBase
	{
		private readonly EnglishLearningDbContext _context;
		private readonly OpenAIClient _openAiClient;
		private readonly ChatClient _chatClient;

		public ConversationsController(
			EnglishLearningDbContext context,
			IOptions<OpenAISettings> openAiOptions) // ← 從 appsettings 注入 API Key
		{
			_context = context;

			var apiKey = openAiOptions.Value.ApiKey; // 從設定取出金鑰
			_openAiClient = new OpenAIClient(apiKey);
			_chatClient = _openAiClient.GetChatClient("gpt-4.1-nano"); // 或改成 gpt-4, gpt-4o
		}

		// 新增對話紀錄 API
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddConversation([FromBody] ConversationCreateDto dto)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var messages = new List<ChatMessage>
			{
				new SystemChatMessage("你是AI英文助教，請回答英文學習相關問題。"),
				new UserChatMessage(dto.Question)
			};

			ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
			var aiResponse = completion.Content[0].Text;

			var conv = new Conversation
			{
				UserId = userId,
				TopicId = dto.TopicId,
				Question = dto.Question,
				Answer = aiResponse,
				CreatedAt = DateTime.Now
			};

			_context.Conversations.Add(conv);
			await _context.SaveChangesAsync();
			return Ok(conv);
		}

		// 查詢對話紀錄 API
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var convs = await _context.Conversations
				.Where(c => c.UserId == userId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();

			return Ok(convs);
		}
	}
}
