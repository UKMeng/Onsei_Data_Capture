using HtmlAgilityPack;
using Serilog;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ODC
{
    class Crawler
    {
        private string url;
        private string workPath;
        private string albumPath;
        private string id { get; set; }
        public string title {get; set;}
        public string seriesName {get; set;}
        public string studioName {get; set;}
        public List<string> actorNames {get; set;}
        public string actorStr = "";
        public List<string> tags {get; set;} = new List<string>();
        public string tagStr = "";
        private string directorName {get; set;}
        private string releaseDate {get; set;}
        public string releaseYear {get; set;}
        private string outline {get; set;}
        private static HttpClient httpClient = new HttpClient();

        public Crawler(string queryPath)
        {  
            workPath = queryPath;
            id = Path.GetFileName(workPath).ToUpper();
            url = "https://www.dlsite.com/pro/work/=/product_id/" + id + ".html";
            Log.Debug($"Create Successfully: {this.id}");
        }

        public static void InitializeHttpClient()
        {
            Log.Logger = Settings.Logger;
            var proxy = new WebProxy(Settings.Proxy);
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("locale", "ja-jp", "/", ".dlsite.com"));
            var handler = new HttpClientHandler(){
                Proxy = proxy,
                UseProxy = Settings.UseProxy,
                CookieContainer = cookies,
                UseCookies = true
            };
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3100.0 Safari/537.36");
            httpClient.Timeout = TimeSpan.FromSeconds(20);
        }
        private static async Task<string> GetHtml(string url)
        {   
            Log.Debug("Test in Crawler");
            string html = string.Empty;
            try
            {
                var response = await httpClient.GetAsync(url);
                var content = response.Content;
                html = await content.ReadAsStringAsync();             
            }
            catch (TaskCanceledException)
            {
                throw new TaskCanceledException("Error: Can't not connect to dlsite.com, please check your network configuration");
            }
            catch (Exception)
            {
                throw;
            }

            return System.Web.HttpUtility.HtmlDecode(html);
        }
        public async Task Start()
        {
            try
            {
                string html = await GetHtml(this.url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                this.title = this.id + " " + GetTitle(htmlDoc);
                this.actorNames = GetActorNames(htmlDoc);
                this.seriesName = GetSeriesName(htmlDoc);
                this.studioName = GetStudioName(htmlDoc);
                GetTags(htmlDoc);
                this.directorName = GetDirectorName(htmlDoc);
                this.releaseDate = GetReleaseDate(htmlDoc);
                this.releaseYear = this.releaseDate.Substring(0, 4);
                this.outline = GetOutline(htmlDoc);               
                GetAlbumPath();
                FileProcessor.MoveAllFilesFromDirectory(this.workPath, this.albumPath);
                OutputNFO();
                FileProcessor.EditTags(this.albumPath, this);
                Log.Debug("Finish: " + this.id);
            }
            catch(Exception)
            {
                throw;
            }
        }
        private string GetTitle(HtmlDocument htmlDoc)
        {
            try
            {
                string retTitle = Regex.Replace(htmlDoc.DocumentNode.SelectNodes("//*[@id=\"work_name\"]/text()").First().InnerText, "【.*?】", "");
                return retTitle;
            }
            catch
            {
                return "";
            }
        }
        private string GetStudioName(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"サークル名\")]/../td/span[1]/a/text()").First().InnerText;
            }
            catch
            {
                return "";
            }
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
        private void GetTags(HtmlDocument htmlDoc)
        {
            try
            {
                foreach(var node in htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"ジャンル\")]/../td/div/a/text()"))
                {
                    //Console.WriteLine(node.InnerText);
                    this.tags.Add(node.InnerText);
                }
                if(studioName != "")
                {
                    this.tags.Add("社团名：" + this.studioName);
                }
                if(seriesName != "")
                {
                    this.tags.Add("系列名：" + this.seriesName);
                }
                foreach(string tag in this.tags)
                {
                    if(tagStr == "")
                    {
                        tagStr = tag.Trim();
                    }
                    else
                    {
                        tagStr = tagStr + ";" + tag.Trim();
                    }
                }
            }
            catch
            {
                Log.Information("Can't find any tag of " + this.id); 
            }
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
        private string GetDirectorName(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"シナリオ\")]/../td/a/text()").First().InnerText;
            }
            catch
            {
                return "";
            }
        }
        private string GetReleaseDate(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode.SelectNodes("//th[contains(text(),\"販売日\")]/../td/a/text()").First().InnerText.Replace("年", "-").Replace("月", "-").Replace("日", "");
            }
            catch
            {
                Log.Information("Can't find release Date of " + this.id);
                return "2022-01-01";
            }
        }
        private string GetOutline(HtmlDocument htmlDoc)
        {
            try
            {
                string outline = htmlDoc.DocumentNode.SelectNodes("//meta[@name=\"description\"]").First().Attributes["content"].Value;
                return Regex.Replace(outline, "「DLsite 同人」は.*", "");
            }
            catch
            {
                return "";
            }
        }
        private void GetAlbumPath()
        {
            try
            {
                foreach(string name in actorNames)
                {
                    if(actorStr == "")
                    {
                        actorStr = name.Trim();
                    }
                    else
                    {
                        actorStr = actorStr + ";" + name.Trim();
                    }
                }
                string actorPath = Path.Join(Settings.OutputDir, actorStr);
                if(!Directory.Exists(actorPath))
                {
                    try
                    {
                        Directory.CreateDirectory(actorPath);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                this.albumPath = Path.Join(Settings.OutputDir, this.actorStr, this.id);
                Log.Debug("Album Path: " + albumPath);
            }
            catch(Exception)
            {
                throw;
            }
        }
        private void OutputNFO()
        {
            try
            {
                Log.Debug("------------");
                Log.Debug("This is information about " + this.id);
                Log.Debug("Actor: " + this.actorStr);
                Log.Debug("Series: " + this.seriesName);
                Log.Debug("Title: " + this.title);
                Log.Debug("Studio: " + this.studioName);
                Log.Debug("Genre: " + this.tagStr);
                Log.Debug("Writer: " + this.directorName);
                Log.Debug("Release Date: " + this.releaseDate);
                Log.Debug("Release Year: " + this.releaseYear);
                Log.Debug("Outline: " + this.outline);
                Log.Debug("------------");
                new XDocument(
                    new XElement("album",
                        new XElement("title", this.title),
                        new XElement("year", this.releaseYear),
                        new XElement("premiered", this.releaseDate),
                        new XElement("releasedate", this.releaseDate),
                        new XElement("outline", this.outline),
                    //  new XElement("plot", this.outline),
                        new XElement("studio", this.studioName),
                        new XElement("director", this.directorName),
                        this.actorNames.Select(name => new XElement("artist", name)),
                        this.actorNames.Select(name => new XElement("albumartist", name)),
                        new XElement("art", 
                            new XElement("poster", "cover.jpg"),
                            new XElement("fanart", "fanart.jpg")
                        ),
                        this.tags.Select(tag => new XElement("genre", tag)),
                        new XElement("maker", this.studioName)
                    )
                ).Save(Path.Join(this.albumPath, "album.nfo"));
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}

// System.IO.File.WriteAllText("./test.html", html);