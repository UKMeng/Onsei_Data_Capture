using Microsoft.Extensions.Configuration;


namespace ODC
{
    public class Settings
    {
        public static string WorkingDir {get; set;}
        public static string OutputDir {get; set;}
        public static string FailedDir {get; set;}
        public static string Proxy {get; set;}
        public static bool UseProxy {get; set;} = false;
        private static IConfiguration config;
        public static void InitializeSettings()
        {
            try
            {
                config = new ConfigurationBuilder()
                .AddIniFile("Config.ini")
                .Build();
                InitCommon();
                InitProxy();
            }
            catch (Exception e)
            {                
                Console.WriteLine(e.Message);
            }
        }
        private static void InitCommon()
        {
            try
            {
                var commonSection = config.GetRequiredSection("common");
                WorkingDir = commonSection.GetRequiredSection("sourceFolder").Value;
                OutputDir = Path.Join(WorkingDir, commonSection.GetRequiredSection("successOutputFolder").Value);
                FailedDir = Path.Join(WorkingDir, commonSection.GetRequiredSection("failedOutputFolder").Value);
                //Console.WriteLine(WorkingDir);
                //Console.WriteLine(OutputDir);
                //Console.WriteLine(FailedDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }           
        }
        private static void InitProxy()
        {
            try
            {
                string prefix = "http://";
                string address = "127.0.0.1:8080";
                var proxySection = config.GetRequiredSection("proxy");
                var typeSection = proxySection.GetRequiredSection("type");
                if(typeSection.Value == "no")
                {
                    UseProxy = false;
                }
                else if(typeSection.Value == "http" || typeSection.Value == "socks5")
                {
                    UseProxy = true;
                    prefix = typeSection.Value + "://";
                }
                else
                {
                    throw new Exception("ERROR: Invalid Type of Proxy");
                }
                var addressSection = proxySection.GetRequiredSection("proxy");
                if(UseProxy) address = addressSection.Value;
                Proxy = prefix + address;               
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }      
    }
}