using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using System.Net.Http.Json;

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

		[HttpGet("BulkCheck")]
		public async Task<ActionResult<IEnumerable<WordDictionary>>> BulkCheck([FromQuery] string[] words)
		{
			try
			{
				if (words == null || words.Length == 0)
					return BadRequest("必須至少提供一個單字，像：?words=cat&words=dog");

				var lowerWords = words
					.Where(w => !string.IsNullOrWhiteSpace(w))
					.Select(w => w.Trim().ToLower())
					.Distinct()
					.ToList();

				var existingList = await _context.WordDictionaries
					.Where(w => lowerWords.Contains(w.Word.ToLower()))
					.ToListAsync();

				var existingDict = existingList.ToDictionary(w => w.Word.ToLower(), w => w);
				var results = new List<WordDictionary>();
				var now = DateTime.UtcNow.AddHours(8); // ✅ 轉成 GMT+8

				foreach (var word in lowerWords)
				{
					if (existingDict.TryGetValue(word, out var existingWord))
					{
						bool needsUpdate = false;

						// ✅ 若超過一天未更新，才重新查詢 API
						if (existingWord.LastUpdated == null || (now - existingWord.LastUpdated).TotalHours >= 24)
						{
							try
							{
								var client = _httpClientFactory.CreateClient();
								var apiUrl = $"https://api.dictionaryapi.dev/api/v2/entries/en/{word}";
								var response = await client.GetAsync(apiUrl);

								if (response.IsSuccessStatusCode)
								{
									var jsonList = await response.Content.ReadFromJsonAsync<List<DictionaryApiResult>>();
									var entry = jsonList?.FirstOrDefault();
									var meaning = entry?.meanings?.FirstOrDefault();
									var def = meaning?.definitions?.FirstOrDefault();

									if (!string.IsNullOrWhiteSpace(def?.definition) && string.IsNullOrWhiteSpace(existingWord.Definition))
									{
										existingWord.Definition = def.definition;
										needsUpdate = true;
									}

									if (!string.IsNullOrWhiteSpace(def?.example) && string.IsNullOrWhiteSpace(existingWord.Example))
									{
										existingWord.Example = def.example;
										needsUpdate = true;
									}

									if (!string.IsNullOrWhiteSpace(meaning?.partOfSpeech) && string.IsNullOrWhiteSpace(existingWord.PartOfSpeech))
									{
										existingWord.PartOfSpeech = meaning.partOfSpeech;
										needsUpdate = true;
									}

									if (!string.IsNullOrWhiteSpace(entry?.phonetic) && string.IsNullOrWhiteSpace(existingWord.Phonetic))
									{
										existingWord.Phonetic = entry.phonetic;
										needsUpdate = true;
									}
								}
							}
							catch (Exception apiErr)
							{
								Console.WriteLine($"❌ 補全失敗 {word}：{apiErr.Message}");
							}
						}

						if (needsUpdate)
						{
							existingWord.LastUpdated = now;
							await _context.SaveChangesAsync();
						}

						results.Add(existingWord);
					}
					else
					{
						try
						{
							var client = _httpClientFactory.CreateClient();
							var apiUrl = $"https://api.dictionaryapi.dev/api/v2/entries/en/{word}";
							var response = await client.GetAsync(apiUrl);

							if (!response.IsSuccessStatusCode)
							{
								Console.WriteLine($"⚠️ API 查無單字: {word}");
								continue;
							}

							var jsonList = await response.Content.ReadFromJsonAsync<List<DictionaryApiResult>>();
							var entry = jsonList?.FirstOrDefault();
							if (entry == null || entry.meanings == null || entry.meanings.Count == 0)
							{
								Console.WriteLine($"⚠️ 資料格式錯誤: {word}");
								continue;
							}

							var meaning = entry.meanings.First();
							var definition = meaning.definitions.FirstOrDefault();

							var newWord = new WordDictionary
							{
								Word = word,
								PartOfSpeech = meaning.partOfSpeech ?? "",
								Definition = definition?.definition ?? "",
								Example = definition?.example ?? "",
								Phonetic = entry.phonetic ?? "",
								Translation = "",
								LastUpdated = now
							};

							_context.WordDictionaries.Add(newWord);
							await _context.SaveChangesAsync();
							results.Add(newWord);
						}
						catch (Exception innerEx)
						{
							Console.WriteLine($"❌ 錯誤單字 {word}：{innerEx.Message}");
							continue;
						}
					}
				}

				return Ok(results);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ BulkCheck 發生未預期錯誤：{ex.Message}");
				return StatusCode(500, $"BulkCheck 發生錯誤：{ex.Message}");
			}
		}
	



// ✅ 額外 API：讓前端取得無效單字清單

[HttpGet("Invalid")]
		public async Task<ActionResult<IEnumerable<string>>> GetInvalidWords()
		{
			return await _context.InvalidWords.Select(x => x.Word).ToListAsync();
		}

		public class DictionaryApiResult
		{
			public string word { get; set; }
			public string phonetic { get; set; }
			public List<Meaning> meanings { get; set; }
		}

		public class Meaning
		{
			public string partOfSpeech { get; set; }
			public List<Definition> definitions { get; set; }
		}

		public class Definition
		{
			public string definition { get; set; }
			public string example { get; set; }
		}

	}
}
