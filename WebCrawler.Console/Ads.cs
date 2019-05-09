using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebCrawler.Console
{
    public class Ads
    {
        protected string _cookie = "";

        protected string _basePath = @"C:\WebCrawler\";

        public string GetHtml(string url, bool setCookie = false)
        {
            string html = "";
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
                if (!setCookie)
                {
                    request.Headers.Add(HttpRequestHeader.Cookie, _cookie);
                }
                WebResponse response = request.GetResponse();
                if (setCookie)
                {
                    var cookie = response.Headers["Set-Cookie"];
                    if (cookie != null) _cookie = cookie;
                }
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    html = reader.ReadToEnd();
                }

                return html;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return html;
            }
        }

        public void SavePicture(string url, string path)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, path);
            }
        }
    }
}