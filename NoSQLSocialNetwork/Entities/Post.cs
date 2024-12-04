using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSocialNetwork.Entities
{
    public class Post
    {
        public Post() {
            Id = ObjectId.GenerateNewId();
           }
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("AuthorId")]
        public ObjectId AuthorId { get; set; }

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

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("comments")]
        public List<Comment>? Comments { get; set; }
    }
}
