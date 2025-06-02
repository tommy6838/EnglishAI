using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Controllers
{
	// 設定 API 路由為 /api/glosbeapi
	[Route("api/[controller]")]
	[ApiController]
	public class GlosbeAPIController : ControllerBase
	{
		private readonly HttpClient _httpClient;

		// 透過建構函式注入 HttpClient（建議在 Program.cs 中註冊）
		public GlosbeAPIController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		// GET /api/glosbeapi/translate?word=apple
		[HttpGet("translate")]
		public async Task<IActionResult> Translate([FromQuery] string word)
		{
			// 組出 Glosbe 的實際查詢網址
			var url = $"https://glosbe.com/gapi/translate?from=en&dest=zh-TW&format=json&phrase={word}";

			try
			{
				// 向 Glosbe 發送 GET 請求
				var response = await _httpClient.GetAsync(url);

				// 讀取回應內容
				var content = await response.Content.ReadAsStringAsync();

				// 回傳原樣 JSON 給前端
				return Content(content, "application/json");
			}
			catch
			{
				// 發生錯誤時回傳 500
				return StatusCode(500, "❌ 翻譯失敗，請稍後再試");
			}
		}
	}
}
