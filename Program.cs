namespace ODC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Settings.InitializeSettings();
            FileProcessor.InitializeFileProcessor();
            Crawler.InitializeHttpClient();
            Console.WriteLine($"The number of queries: {FileProcessor.Queries.Count}");
            int cnt = 0;
            foreach(var query in FileProcessor.Queries)
            {
                Console.WriteLine($"Process: {cnt} / {FileProcessor.Queries.Count}");
                Crawler test = new Crawler(query);
                await test.Start();
                cnt++;
                test.Test();
            }    
            // Console.WriteLine(test.Proxy);
            // https://www.dlsite.com/home/work/=/product_id/RJ363741.html
        }
    }
}
