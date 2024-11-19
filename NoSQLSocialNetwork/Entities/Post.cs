using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQLSocialNetwork.Entities
{
    public class Post
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("content")]
        public string? Content { get; set; }

        [BsonElement("imageUrls")]
        public List<string>? ImageUrls { get; set; }

        [BsonElement("likes")]
        public List<ObjectId>? Likes { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updateAt")]
        public DateTime UpdateAt { get; set; }

        [BsonElement("comments")]
        public List<Comment>? Comments { get; set; }
    }
}
