using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.Data;   // 引入你的 DbContext 命名空間
using EnglishLearningAPI.Models; // 引入你的 Model 命名空間
using Microsoft.EntityFrameworkCore; // 為了使用 ToListAsync()

namespace EnglishLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly EnglishLearningDbContext _context;

        public TopicsController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        //GET API/Topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
        {
            // 從資料庫抓取所有主題
            var topics = await _context.Topics.ToListAsync();
            return Ok(topics);
		}
	}
}
