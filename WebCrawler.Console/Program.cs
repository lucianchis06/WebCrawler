using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebCrawler.Database;
using WebCrawler.Database.Models;

namespace WebCrawler.Console
{
    //string urlToCheck = "https://www.olx.ro/auto-masini-moto-ambarcatiuni/autoturisme/?page=" + pageNumber;
    //        HttpWebRequest request = HttpWebRequest.Create(urlToCheck) as HttpWebRequest;
    //        request.Method = "GET";

    //        /* Sart browser signature */
    //        request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
    //        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
    //        request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
    //        /* Sart browser signature */

    //        WebResponse response = request.GetResponse();
    //        using (Stream stream = response.GetResponseStream())
    //        {
    //            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
    //            String responseString = reader.ReadToEnd();
    //        }


    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Crawler is running!");
            StartCrawler();
            System.Console.ReadLine();
        }

        private static void StartCrawler()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer("Data Source=ISS0;Database=WebCrawler;User ID=sa;PWD=Afpftcb1td;Trusted_Connection=False;");
            var context = new DatabaseContext(builder.Options);

            AutovitAds autovit = new AutovitAds(context);
            OlxAds olx = new OlxAds(context);

            while (true)
            {
                //autovit.ProcessTable();

                olx.ProcessTable();

                System.Console.WriteLine("");
                System.Console.WriteLine("Service is sleeping");
                System.Console.WriteLine("");
                Thread.Sleep(new TimeSpan(0, 30, 0));
            }
        }
    }
}
