using System.Threading;
using ChatroomBot;

namespace PingPongBot
{
    class Program
    {
        static void Main(string[] args)
        {
            PingPongBot bot = new PingPongBot("127.0.0.1", 25565);

            while (true)
            {
                bot.Update();
                Thread.Sleep(1000);
            }
        }
    }

    class PingPongBot : Bot
    {
        public PingPongBot(string hostname, ushort port) : base("Ping pong", hostname, port)
        {
        }

        protected override void OnMessage(Message message)
        {
            if (message.Content.ToLower().Contains("ping"))
            {
                Send("pong!");
            }
        }
    }
}
