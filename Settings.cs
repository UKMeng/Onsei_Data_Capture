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
                foreach(var child in config.GetRequiredSection("proxy").GetChildren())
                {
                    if(child.Key == "type")
                    {
                        if(child.Value == "no")
                        {
                            UseProxy = false;
                        }
                        else if(child.Value == "http" || child.Value == "socks5")
                        {
                            UseProxy = true;
                            prefix = child.Value + "://";
                        }
                        else
                        {
                            throw new Exception("ERROR: Invalid Type of Proxy");
                        }
                    }
                    else
                    {
                        address = child.Value;
                    }
                }
                Proxy = prefix + address;               
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }      
    }
}