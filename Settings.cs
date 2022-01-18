using Microsoft.Extensions.Configuration;


namespace ODC
{
    public class Settings
    {
        public string Proxy {get; set;} = null!;
    }

/*
public class GetSettings
{
    public Settings test;

    public static GetSettings
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("App.config")
            .Build();
        Settings test = config.GetRequiredSection("Settings").Get<Settings>();    
    }
}
*/
}