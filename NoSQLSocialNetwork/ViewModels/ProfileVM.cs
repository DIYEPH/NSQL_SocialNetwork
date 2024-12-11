using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NoSQLSocialNetwork.Entities;

namespace NoSQLSocialNetwork.ViewModels
{
    public class ProfileVM
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }

        public string? AvatarUrl { get; set; }
        public DateTime Birthday { get; set; }

        public List<ObjectId>? Followers { get; set; }

        public List<ObjectId>? Followings { get; set; }


        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<Post>? Posts { get; set; }
    }
}
