using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQLSocialNetwork.Entities
{
    public class Message
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("isGroupChat")]
        public bool IsGroupChat { get; set; }

        [BsonElement("adminGroup")]
        public ObjectId AdminGroup { get; set; }

        [BsonElement("image")]
        public string? Image { get; set; }

        [BsonElement("content")]
        public string? Content { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("participants")]
        public List<ObjectId>? Participants { get; set; }

        [BsonElement("messages")]
        public List<MessageContent>? Messages { get; set; }
    }
}
