using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
