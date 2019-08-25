namespace RoleBot
{
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            var config = Configuration.Config.Load();
            if (config == null)
            {
                Console.WriteLine($"Failed to load config file '{Config.ConfigFileName}'.");
                return;
            }

            var bot = new Bot(config);
            await bot.Start();

            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }
    }
}