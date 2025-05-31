using System;

namespace EnglishLearningAPI.Models
{
	public partial class User
	{
		public User()
		{
			// 如果 Id 為 null 或空字串才自動產生（避免 Scaffold 時不小心填過了）
			if (string.IsNullOrWhiteSpace(Id))
				Id = Guid.NewGuid().ToString();

			// 如果 RegisteredAt 是預設值，才填入現在時間（避免被資料庫預設值取代）
			if (RegisteredAt == default)
				RegisteredAt = DateTime.Now;
		}
	}
}
