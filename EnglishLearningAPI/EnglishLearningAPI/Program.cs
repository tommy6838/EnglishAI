using Microsoft.EntityFrameworkCore;
using EnglishLearningAPI.Data; // �o�@��̷ӧA��DbContext�Ҧb�R�W�Ŷ�
//�U�T��JWT��
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ���UDbContext�]��Ʈw�^
builder.Services.AddDbContext<EnglishLearningDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//"OpenAI KEY"
builder.Services.Configure<OpenAISettings>(
	builder.Configuration.GetSection("OpenAI"));


// �[�JJWT�{�ҪA�ȡ]�o�q�����^��������
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
				Encoding.UTF8.GetBytes("�A���K�_�г]�ܪ��ܶ�"))
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
//���ѨMCORS���D

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT�@�w�n�[
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost5173"); // �ҥΤW�����ۭq�� CORS �W�h

app.UseAuthentication(); // <==== JWT�o�@��@�w�n���I�ӥB�bAuthorization���e
app.UseAuthorization();

app.MapControllers();

app.Run();
