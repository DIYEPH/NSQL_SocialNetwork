using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSocialNetwork.Entities
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("passwordHash")]
        public string? PasswordHash { get; set; }

        [BsonElement("fullname")]
        public string? FullName { get; set; }

        [BsonElement("avatarUrl")]
        public string? AvatarUrl { get; set; }

        [BsonElement("birthday")]
        public DateTime Birthday { get; set; }

        [BsonElement("followers")]
        public List<ObjectId>? Followers { get; set; }

        [BsonElement("followings")]
        public List<ObjectId>? Followings { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("role")]
        public string? Role { get; set; } // "admin" or "user"

        [BsonElement("createAt")]
        public DateTime CreateAt { get; set; }

        [BsonElement("updateAt")]
        public DateTime UpdateAt { get; set; }

        // Các Post sẽ được ánh xạ vào User nếu cần
        [BsonElement("posts")]
        public List<Post>? Posts { get; set; } 
    }


}
