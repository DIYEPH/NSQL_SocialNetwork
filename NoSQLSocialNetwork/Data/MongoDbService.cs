﻿using MongoDB.Driver;
using NoSQLSocialNetwork.Entities;

namespace NoSQLSocialNetwork.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("DbConnection");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase("SocialNetwork");
        }
        public IMongoDatabase? Database => _database;

    }
}
