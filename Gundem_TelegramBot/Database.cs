using System;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Gundem_TelegramBot
{
    public interface IDatabase{
        void AddFollowedTitle(string chat_id, string title);
        void AddFollowedUser(string chat_id, string user);
    
    }

    public class DummyDatabase : IDatabase
    {
        public void AddFollowedTitle(string chat_id, string title)
        {
            Console.WriteLine($"Following title.  chat_id: {chat_id}, title: {title}");
        }

        public void AddFollowedUser(string chat_id, string user)
        {
            Console.WriteLine($"Following user. chat_id: {chat_id}, user: {user}");
        }

    }

    public class MongoDatabase : IDatabase
    {
        private readonly string connectionString;
        private readonly IMongoCollection<BsonDocument> _titles;
        private readonly IMongoCollection<BsonDocument> _users;
        public MongoDatabase(IConfiguration configuration)
        {
            connectionString = configuration["MongoConnectionString"];
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("telegram");
            _titles = database.GetCollection<BsonDocument>("followTitles");
            _users = database.GetCollection<BsonDocument>("followUsers");
        }

        public void AddFollowedTitle(string chat_id, string title)
        {
            _titles.InsertOne(new BsonDocument
                {
                    { "chat_id",  chat_id},
                    { "last_receivedDate", DateTime.Now},
                    { "follow_title", title},
                });
        }

        public void AddFollowedUser(string chat_id, string user)
        {
            _users.InsertOne(new BsonDocument
                {
                    { "chat_id",  chat_id},
                    { "last_receivedDate", DateTime.Now},
                    { "follow_user", user },
                });
        }

    }

}
