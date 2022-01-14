using System.Net;
using HtmlAgilityPack;

namespace Crawler
{
    /// <summary>
    /// 
    /// </summary>
    class DLsite
    {
        
        public DLsite(string url)
        {
            var html = getHtml(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            
        }
        private static string getHtml(string url)
        {   
            string html = string.Empty;
            try
            {
                var proxy = new WebProxy("127.0.0.1:8080");
                var headers = new WebHeaderCollection();
                headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            }
            catch (WebException ex)
            { 
                Console.WriteLine(ex);
            }
            return html;
        }

    }
}