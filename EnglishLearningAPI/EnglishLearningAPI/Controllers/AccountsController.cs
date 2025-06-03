using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // 提供 PasswordHasher
using EnglishLearningAPI.Models;
using Microsoft.Win32;
using Microsoft.Data.SqlClient;
using EnglishLearningAPI.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace EnglishLearningAPI.Controllers
{
	// 1. 先宣告DTO（資料傳輸物件）類別

	public class RegisterDto
    {
		public string Email { get; set; }     // 註冊用的Email
		public string Password { get; set; }  // 註冊用的密碼
	}

	public class LoginDto
	{
		//public string Email { get; set; }     // 註冊用的Email
		public string LoginKey { get; set; }  // 可以是email也可以是帳號
		public string Password { get; set; }  // 註冊用的密碼
	}

	// 2. Controller本體

	[Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
		//private readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=EnglishLearningDB;Trusted_Connection=True;";

		// ======= EF 寫法：注入DbContext =======
		private readonly EnglishLearningDbContext _db;
		private readonly IConfiguration _config; // 用來取得appsettings的JWT密鑰

		public AccountsController(EnglishLearningDbContext db, IConfiguration config)
		{
			_db = db;
			_config = config; //JWT
		}

		// ========== 註冊API ==========
		[HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
			// 1. 檢查Email有沒有被註冊過
			if (_db.Users.Any(u => u.Email == dto.Email))
				return BadRequest("此Email已被註冊！");

			// 從Email自動取userName
			string UserName = dto.Email.Split('@')[0];

			// 2. 用 PasswordHasher 將密碼做安全雜湊
			var hasher = new PasswordHasher<User>();
			var fakeUser = new User();
			// PasswordHasher 需要一個User物件
			string hash = hasher.HashPassword(fakeUser, dto.Password);

			// 3. 建立新User物件
			var user = new User
			{
				Email = dto.Email,
				PasswordHash = hash,
				Level = 1,
				UserName = UserName // 自動產生
			};

			// 4. 寫入資料庫
			_db.Users.Add(user);
			_db.SaveChanges();

			// 5. 回傳成功
			return Ok("註冊成功");
		}

		// ========== 登入API ==========
		[HttpPost("Login")]
		public IActionResult Login([FromBody] LoginDto dto)
		{
			// 1. 查詢是否有這個 Email 的帳號
			var user = _db.Users.SingleOrDefault(
				u => u.Email == dto.LoginKey || u.UserName == dto.LoginKey);

			if (user == null)
				return Unauthorized("帳號或密碼錯誤。");

			// 2. 用 PasswordHasher 驗證密碼
			var hasher = new PasswordHasher<User>();
			var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
			if (result == PasswordVerificationResult.Failed)
				return Unauthorized("帳號或密碼錯誤。");

			// 3. 登入成功，回傳資訊
			//return Ok(new { Message = "登入成功", user.Id, LogLevel = user.Level });

			// 產生JWT Token
			var token = GenerateJwtToken(user);

			// 回傳token給前端
			return Ok(new
			{
				token, // JWT token
				user.Id,
				Level = user.Level
			});
		}

		// ========== 產生JWT Token的方法 ==========
		// 建議放在Controller最後面，讓API邏輯一目了然
		private string GenerateJwtToken(User user)
		{
			var jwtKey = _config["JwtSettings:SecretKey"]; // 從appsettings.json取key

			// 建立claims，可以加自己想要的內容
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("Level", user.Level.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddDays(7), // 有效7天
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
