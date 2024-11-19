using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSocialNetwork.Entities
{
    public class Comment
    {
        [BsonElement("userId")]
        public ObjectId UserId { get; set; }

        [BsonElement("content")]
        public string? Content { get; set; }

        [BsonElement("createAt")]
        public DateTime CreateAt { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }
    }
}
