using System;
using System.IO;
using System.Threading.Tasks;
using ChatroomBot;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DelDetBot
{
    public class DelDenBot : Bot
    {
        DiscordSocketClient client;

        public DelDenBot(string hostname, ushort port) : base("DelDet", hostname, port)
        {
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += Client_MessageReceived;

            var token = File.ReadAllText("Token.txt");
            await client.LoginAsync(Discord.TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(300);

            foreach (var guild in client.Guilds)
            {
                Console.Write($"{guild} ");
            }
            Console.WriteLine();

            await Task.Delay(-1);
        }


        protected override void OnConnectionFinished(bool connected)
        {
            if (connected)
            {
                Console.WriteLine("Connected");
            }
        }

        private async Task Client_MessageReceived(SocketMessage e)
        {
            var message = e as SocketUserMessage;
            var context = new SocketCommandContext(client, message);

            if (e.Channel.Id != 789171198646943785)
            {
                return;
            }

            if (e.Author.Id != 789177187529654332)
            {
                return;
            }

            if (e.Content == "Johnny del det")
            {
                return;
            }

            if (e.Content == string.Empty)
            {
                Send("[Billede]");
            }
            else
            {
                Send(e.Content);
            }
        }

        protected override async void OnMessage(Message message)
        {
            if (message.Author.IsSelf || message.Author == Member.Server)
            {
                return;
            }

            if (message.CreatedAt < DateTime.Now.Subtract(TimeSpan.FromSeconds(10)))
            {
                return;
            }

            SocketGuild guild = null;

            foreach (var item in client.Guilds)
            {
                if(item.Id == 789170799461400577)
                {
                    guild = item;
                    break;
                }
            }

            if (guild is null)
            {
                return;
            }

            var channel = guild.GetTextChannel(789171198646943785);
            await channel.SendMessageAsync(message.Content);
        }
    }
}
