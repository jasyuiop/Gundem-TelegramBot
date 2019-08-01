using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using HtmlAgilityPack;

namespace Gundem_TelegramBot
{
    class Program
    {
        public static ITelegramBotClient botClient;

        static void Main()
        {
            botClient = new TelegramBotClient("***REMOVED***");

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Id = {me.Id} | Name = {me.FirstName}. \n"
            );

            botClient.OnMessage += Message.Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }
    }
}