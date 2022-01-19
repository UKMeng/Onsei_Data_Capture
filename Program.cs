namespace ODC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Settings.InitializeSettings();
            Crawler.InitializeHttpClient();
            Crawler test = new Crawler("RJ361906");
            await test.Start();
            test.Test();
            test.OutputXml();
            // Console.WriteLine(test.Proxy);
            // https://www.dlsite.com/home/work/=/product_id/RJ363741.html
        }
    }
}
