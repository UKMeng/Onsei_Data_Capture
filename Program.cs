using Serilog;

namespace ODC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Settings.InitializeSettings();
                Log.Logger = Settings.Logger;             
                FileProcessor.InitializeFileProcessor();
                Crawler.InitializeHttpClient();
                Log.Information($"The number of queries: {FileProcessor.Queries.Count}");
                int cnt = 0;
                foreach(var query in FileProcessor.Queries)
                {
                    Log.Information($"Process: {cnt} / {FileProcessor.Queries.Count}");
                    Crawler test = new Crawler(query);
                    await test.Start();
                    cnt++;
                }    
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Debug(e.StackTrace);
            }
            
            // Console.WriteLine(test.Proxy);
            // https://www.dlsite.com/home/work/=/product_id/RJ363741.html
        }
    }
}
