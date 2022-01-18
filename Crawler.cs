using HtmlAgilityPack;
using System.Net;

namespace ODC
{
    class Crawler
    {
        private string url;
        public string seriesName {get; set;} = "";
        public List<string> actorNames {get; set;} = new List<string>();

        private static HttpClient httpClient = new HttpClient();

        public Crawler(string number)
        {  
            number = number.ToUpper();
            url = "https://www.dlsite.com/pro/work/=/product_id/" + number + ".html";
            Console.WriteLine("Create Successfully");
        }

        public static void InitializeHttpClient(string proxyUrl)
        {
            var proxy = new WebProxy(proxyUrl);
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("locale", "ja-jp", "/", ".dlsite.com"));
            var handler = new HttpClientHandler(){
                Proxy = proxy,
                UseProxy = true,
                CookieContainer = cookies,
                UseCookies = true
            };
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3100.0 Safari/537.36");
        }
        private static async Task<string> GetHtml(string url)
        {   
            string html = string.Empty;
            try
            {
                //var request = WebRequest.Create(url);
                var response = await httpClient.GetAsync(url);
                var content = response.Content;
                html = await content.ReadAsStringAsync();             
            }
            catch (WebException ex)
            { 
                Console.WriteLine(ex);
            }

            return html;
        }
        public async Task Start()
        {
            string html = await GetHtml(this.url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            this.actorNames = GetActorNames(htmlDoc);
            this.seriesName = GetSeriesName(htmlDoc);
            foreach(var name in this.actorNames)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine(this.seriesName);
        }

        private List<string> GetActorNames(HtmlDocument htmlDoc)
        {
            List<string> retNames = new List<string>();
            try
            {
                foreach(var node in htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"声優\")]/../td/a/text()"))
                {
                    //Console.WriteLine(node.InnerText);
                    retNames.Add(node.InnerText);
                }
            }
            catch
            {
                retNames.Add("unknown");
            }            
            return retNames;
        }
        private string GetSeriesName(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"シリーズ名\")]/../td/a/text()").First().InnerText;
            }
            catch
            {
                return "";
            }
        }
    }
}

// System.IO.File.WriteAllText("./test.html", html);