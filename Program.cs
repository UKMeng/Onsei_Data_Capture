using Microsoft.Extensions.Configuration;

namespace ODC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("App.config")
                .Build();
            Settings configuration = config.GetRequiredSection("Settings").Get<Settings>();
            Crawler.InitializeHttpClient(configuration.Proxy);
            Crawler test = new Crawler("https://www.dlsite.com/home/work/=/product_id/RJ363741.html");
            await test.Start();
            // Console.WriteLine(test.Proxy);
            // https://www.dlsite.com/home/work/=/product_id/RJ363741.html
        }
    }
}
