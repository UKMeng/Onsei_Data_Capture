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
                int cnt = 1;
                foreach(var query in FileProcessor.Queries)
                {
                    Log.Information($"Progress: {cnt} / {FileProcessor.Queries.Count}");
                    Log.Information($"Processing: {Path.GetFileName(query)}");
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
