using System;
using MongoDB.Driver;
using MongoDB.Bson;
namespace Gundem_TelegramBot
{
    public class Database
    {
        static string connectionString = ""; // mongodb Connection String
        public static void Insertfollow_Title(string chat_id, DateTime last_receivedDate, string follow_title)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("telegram");
            var collection = database.GetCollection<BsonDocument>("followTitles");
            var document = new BsonDocument
                {
                    { "chat_id",  chat_id},
                    { "last_receivedDate", last_receivedDate},
                    { "follow_title", follow_title },
                };
            collection.InsertOne(document);

        }
        public static void Insertfollow_User(string chat_id, DateTime last_receivedDate, string follow_user)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("telegram");
            var collection = database.GetCollection<BsonDocument>("followUsers");
            var document = new BsonDocument
                {
                    { "chat_id",  chat_id},
                    { "last_receivedDate", last_receivedDate},
                    { "follow_user", follow_user },
                };
            collection.InsertOne(document);
        }
    }

}
