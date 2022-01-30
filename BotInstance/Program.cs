using System.Threading;
using System.Threading.Tasks;
using DelDetBot;

namespace BotInstance
{
    class Program
    {
        static void Main(string[] args)
        {
            DelDenBot bot = new DelDenBot("10.242.79.240", 25565);

            var task = Task.Run(async () => await bot.RunBotAsync());

            while (true)
            {
                bot.Update();
                Thread.Sleep(500);
            }
        }
    }
}
