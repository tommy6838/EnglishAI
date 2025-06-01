using Microsoft.EntityFrameworkCore;
using EnglishLearningAPI.Data; // 這一行依照你的DbContext所在命名空間
//下三行JWT用
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 註冊DbContext（資料庫）
builder.Services.AddDbContext<EnglishLearningDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//"OpenAI KEY"
builder.Services.Configure<OpenAISettings>(
	builder.Configuration.GetSection("OpenAI"));


// 加入JWT認證服務（這段公版）↓↓↓↓
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes("你的密鑰請設很長很亂"))
		};
	});

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
//↑解決CORS問題

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT一定要加
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost5173"); // 啟用上面剛剛自訂的 CORS 規則

app.UseAuthentication(); // <==== JWT這一行一定要有！而且在Authorization之前
app.UseAuthorization();

app.MapControllers();

app.Run();
