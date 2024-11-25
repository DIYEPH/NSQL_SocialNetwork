﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSocialNetwork.Entities
{
    public class MessageContent
    {
        [BsonElement("id")]
        public ObjectId Id { get; set; }

        [BsonElement("content")]
        public string? Content { get; set; }

        [BsonElement("senderId")]
        public ObjectId SenderId { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }
}
