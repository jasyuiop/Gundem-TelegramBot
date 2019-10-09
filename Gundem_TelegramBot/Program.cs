using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using Telegram.Bot;

namespace Gundem_TelegramBot
{
    internal class Program
    {
        public static ITelegramBotClient botClient;
        private static AutoResetEvent autoResetEvent;

        private static void WaitForCancel()
        {
            autoResetEvent = new AutoResetEvent(false);
            Console.WriteLine("Cikmak icin CTRL+Cye basin...");
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                autoResetEvent.Set();
            };
            autoResetEvent.WaitOne();
        }

        private static void Main()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var token = config["TelegramToken"];
            botClient = new TelegramBotClient(config["TelegramToken"]);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Id = {me.Id} | Name = {me.FirstName}.");
            IDatabase database;
            if (config["Veritabani"].ToLower() == "mongo")
                database = new MongoDatabase(config);
            else
                database = new DummyDatabase();
            Message message = new Message(database);
            botClient.OnMessage += message.Bot_OnMessage;
            botClient.StartReceiving();
            WaitForCancel();
        }
    }
}