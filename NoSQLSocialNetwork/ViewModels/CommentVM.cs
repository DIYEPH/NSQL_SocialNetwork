using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQLSocialNetwork.ViewModels
{
    public class CommentVM
    {
        public ObjectId UserId { get; set; }

        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Content { get; set; }

        public DateTime CreateAt { get; set; }
        public int Status { get; set; }
    }
}
