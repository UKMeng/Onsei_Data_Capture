using Microsoft.Extensions.Configuration;


namespace ODC
{
    public class Settings
    {
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
                InitProxy();
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
                var typeSection = config.GetRequiredSection("proxy").GetRequiredSection("type");
                if(typeSection.Key == "type")
                {
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
                }
                var addressSection = config.GetRequiredSection("proxy").GetRequiredSection("proxy");
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