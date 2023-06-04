using System.Threading;
using System.Threading.Tasks;

namespace DelDetBot
{
    class Program
    {
        static void Main(string[] args)
        {
            DelDenBot bot = new DelDenBot("192.168.1.10", 25565);

            var task = Task.Run(async () => await bot.RunBotAsync());

            while (true)
            {
                bot.Update();
                Thread.Sleep(500);
            }
        }
    }
}
