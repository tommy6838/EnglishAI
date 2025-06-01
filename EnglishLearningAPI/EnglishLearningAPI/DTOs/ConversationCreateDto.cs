namespace EnglishLearningAPI.DTOs // ← 命名空間可以和資料夾相同
{
	public class ConversationCreateDto
	{
		public string UserId { get; set; }
		public int TopicId { get; set; }
		public string Question { get; set; }
		//public string Answer { get; set; }
	}
}
