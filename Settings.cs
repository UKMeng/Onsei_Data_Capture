using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

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
        public static Serilog.Core.Logger Logger;
        public static void InitializeSettings()
        {
            try
            {
                config = new ConfigurationBuilder()
                .AddIniFile("Config.ini")
                .Build();
                InitCommon();
                InitLog();
                InitProxy();
            }
            catch (Exception)
            {                
                throw;
            }
        }
        private static void InitLog()
        {
            try
            {
                Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                    .WriteTo.File(Path.Join(System.AppContext.BaseDirectory, $"log/log-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt"), outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (System.Exception)
            {
                throw new Exception("ERROR: Failed to initialize the logger");
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
            catch (Exception)
            {
                throw new FileLoadException("ERROR: Can't set the common configuration");
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
            catch(Exception)
            {
                throw;
            }
        }      
    }
}