using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BlacklistBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            // Logging - Config
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            // Events
            _client.MessageReceived += BanBlacklisted;
            _client.Log += Log;
            _client.MessageReceived += LogToServer;
            _client.MessageUpdated += LogUpdatedMsgToServer;

            // Register the commands
            await RegisterCommandsAsync();

            // Read token from file
            string botToken = File.ReadAllText("Resources\\token.txt");

            // Attempt to login with token
            try
            {
                await _client.LoginAsync(TokenType.Bot, botToken);
            }

            catch
            {
                Console.WriteLine("Unable to login with token.");
                System.Threading.Thread.Sleep(5000);
                return;
            }

            // Attempt to start the client
            try
            {
                await _client.StartAsync();
            }

            catch
            {
                Console.WriteLine("Unable to start client.");
                System.Threading.Thread.Sleep(5000);
                return;
            }

            try
            {
                await Task.Delay(3000);
                Console.Write("Guilds: ");
                foreach (var guild in _client.Guilds)
                {
                    System.Console.Write($"{guild} ");
                }
                Console.Write("\n");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // Keep the client running
            await Task.Delay(-1);
        }

        private async Task LogUpdatedMsgToServer(Cacheable<IMessage, ulong> arg1, SocketMessage msg, ISocketMessageChannel arg3)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\Blacklist\Blacklist\DB_Blacklist.mdf;Integrated Security=True;Connect Timeout=30");
            var message = msg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            var author = msg.Author;
            string authorName = $"{author.Username}#{author.Discriminator}";
            var channel = message.Channel;
            var guild = context.Guild;
            string guildName = $"{guild.Name}";

            if (msg.Author.IsBot)
            {
                return;
            }

            else if (msg.Author.Id != (364497348728061962) && msg.Author.Id != (126323352716574720))
            {
            try
            {
                await connection.OpenAsync();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "insert into [Logs] (Guild_Name,Guild_Id,Channel_Name,Channel_Id,Author,Author_Id,Message,Message_Id) values ('" + guildName + "','" + guild.Id + "','" + channel.Name + "','" + channel.Id + "','" + authorName + "','" + author.Id + "','" + message.Content + "','" + message.Id + "')";
                await cmd.ExecuteNonQueryAsync();
                connection.Close();
            }

            catch
            {
                return;
            }
            }
        }

        private async Task LogToServer(SocketMessage msg)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\Blacklist\Blacklist\DB_Blacklist.mdf;Integrated Security=True;Connect Timeout=30");
            var message = msg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            var author = msg.Author;
            string authorName = $"{author.Username}#{author.Discriminator}";
            var channel = message.Channel;
            var guild = context.Guild;
            string guildName = $"{guild.Name}";

            if (msg.Author.IsBot)
            {
                return;
            }

            else if (msg.Author.Id != (364497348728061962) && msg.Author.Id != (126323352716574720))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "insert into [Logs] (Guild_Name,Guild_Id,Channel_Name,Channel_Id,Author,Author_Id,Message,Message_Id) values ('" + guildName + "','" + guild.Id + "','" + channel.Name + "','" + channel.Id + "','" + authorName + "','" + author.Id + "','" + message.Content + "','" + message.Id + "')";
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }

                catch
                {
                    return;
                }
            }
        }

        // Ban blacklisted users in blacklist.txt (blacklist.txt acts as a database)
        private async Task BanBlacklisted(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            var filePath = "Resources\\blacklist.txt";

            List<Blacklist.Blacklist> users = new List<Blacklist.Blacklist>();

            List<string> lines = File.ReadAllLines(filePath).ToList();

            foreach (var line in lines)
            {
                string[] entries = line.Split(',');

                Blacklist.Blacklist newUser = new Blacklist.Blacklist
                {
                    Username = entries[0],

                    Id = entries[1]
                };

                users.Add(newUser);
            }

            foreach (var user in users)
            {
                try
                {
                    await context.Guild.AddBanAsync(ulong.Parse(user.Id));
                }

                catch
                {
                    return;
                }
            }
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            // Check for bot prefix and do some stuff
            if (message.HasStringPrefix("§", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                // Upon error write to console and current channel
                if (!result.IsSuccess)
                    try
                    {
                        Console.WriteLine($"Invoker -> [{msg.Author}] {result.ErrorReason}");
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }

                    catch
                    {
                        return;
                    }
            }
        }
    }
}
