using MongoDB.Bson;
using NoSQLSocialNetwork.Entities;

namespace NoSQLSocialNetwork.ViewModels
{
	public class PostVM
	{
		public ObjectId Id { get; set; }

		public string? Content { get; set; }

		public List<string>? ImageUrls { get; set; }
        public List<ObjectId>? Likes { get; set; }
		public List<Comment>? Comments { get; set; }

		public ObjectId AuthorId { get; set; }
		public string? AuthorAvatar { get; set; }
		public string? AuthorName { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int Status { get; set; }
	}
}
