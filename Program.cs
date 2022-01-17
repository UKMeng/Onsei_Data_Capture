using Config;
using Crawler;
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
            OnseiCrawler crawler = new OnseiCrawler(configuration.Proxy);
            // await crawler.HtmlParser("https://www.dlsite.com/home/work/=/product_id/RJ363741.html");
            // Console.WriteLine(test.Proxy);
            // https://www.dlsite.com/home/work/=/product_id/RJ363741.html
        }
    }
}
