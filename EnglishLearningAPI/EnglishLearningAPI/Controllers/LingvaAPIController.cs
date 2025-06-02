using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LingvaAPIController : ControllerBase
	{
		private readonly HttpClient _httpClient;

		public LingvaAPIController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		[HttpGet("translate")]
		public async Task<IActionResult> Translate([FromQuery] string word)
		{
			var url = $"https://lingva.ml/api/v1/en/zh/{word}";
			try
			{
				var res = await _httpClient.GetAsync(url);
				var content = await res.Content.ReadAsStringAsync();
				return Content(content, "application/json");
			}
			catch
			{
				return StatusCode(500, "Lingva 備援翻譯失敗");
			}
		}
	}
}
