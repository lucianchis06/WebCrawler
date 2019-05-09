using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WebCrawler.Console;
using WebCrawler.Database.Models;

namespace WebCrawler.Database
{
    public class OlxAds : Ads
    {
        private DatabaseContext _context;

        public OlxAds(DatabaseContext context)
        {
            _context = context;
        }

        public void ProcessTable()
        {
            int counter = 0;
            string link;
            try
            {
                for (var page = 1; page <= 500; page++)
                {
                    System.Console.WriteLine("Page " + page);
                    var url = "https://www.olx.ro/auto-masini-moto-ambarcatiuni/autoturisme/?page=" + page;
                    var html = GetHtml(url, true);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    var carTable = htmlDocument.DocumentNode.Descendants("table").Where(s => s.GetAttributeValue("id", "") == "offers_table");

                    var rows = carTable.FirstOrDefault().Descendants("tr").Where(s => s.GetAttributeValue("class", "") == "wrap");

                    foreach (var row in rows)
                    {
                        try
                        {
                            link = row.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
                            link = link.Split("#")[0];
                            System.Console.WriteLine(link);

                            if (link.Contains("autovit")) continue;

                            if (_context.Ads.Any(s => s.Url == link))
                            {
                                counter++;
                                System.Console.WriteLine("Found");
                                if (counter >= 50)
                                {
                                    page = 501;
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                counter = 0;
                            }

                            ProcessAd(link);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        public void ProcessAd(string link)
        {
            try
            {
                var carHtml = GetHtml(link, true);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(carHtml);
                var div = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("class", "").Contains("descriptioncontent")).FirstOrDefault();
                var items = div.Descendants("table").Where(s => s.GetClasses().Contains("item"));

                Ad a = new Ad();
                a.ModifiedDate = a.CreatedDate = DateTime.UtcNow;
                a.Url = link;
                var description = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("id", "").Contains("textContent")).FirstOrDefault();
                a.Description = description != null ? description.InnerText : "";
                var address = htmlDoc.DocumentNode.Descendants("a").Where(s => s.GetAttributeValue("class", "").Contains("show-map-link")).FirstOrDefault();
                a.Address = address != null ? address.InnerText : "";
                var addedOn = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("class", "").Contains("offer-titlebox__details")).FirstOrDefault();
                var addedOn2 = addedOn.SelectSingleNode("./em[1]/a[1]");
                if (addedOn2 != null && addedOn2.NextSibling != null)
                {
                    a.AddedOnline = addedOn2.NextSibling.InnerText;
                }
                var mainPhoto = htmlDoc.DocumentNode.Descendants("img").Where(s => s.GetAttributeValue("class", "").Contains("bigImage")).FirstOrDefault();
                a.PhotoUrl = mainPhoto != null ? mainPhoto.GetAttributeValue("src", "") : "";
                var phoneToken = htmlDoc.DocumentNode.Descendants("script").Where(s => s.InnerText.Contains("var phoneToken = ")).FirstOrDefault();
                //if (phoneToken != null)
                //{
                //    var token = phoneToken.InnerText.Replace("var phoneToken = ", "").Trim();
                //    token = Regex.Replace(token, @"\t|\n|\r|'|;", "");
                //    var id = link.Remove(0, link.IndexOf("-ID") + 3);
                //    id = id.Remove(id.IndexOf("."), id.Length - id.IndexOf("."));
                //    var resp1 = GetHtml("https://www.olx.ro/ajax/misc/contact/phone/" + id + "/?pt=" + token);
                //    resp1 = resp1.Replace("{\"value\":\"", "").Replace("\"}", "");
                //    a.Phone1 = resp1;
                //    System.Console.WriteLine(resp1);
                //}
                var price = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetClasses().Contains("price-label")).FirstOrDefault();
                a.Price = price != null ? Regex.Replace(price.InnerText, @"\t|\n|\r|'|;", "") : "";
                var userDetails = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetClasses().Contains("offer-user__details")).FirstOrDefault();
                var urlDetails = userDetails.SelectSingleNode("./h4[1]/a[1]");
                if (urlDetails != null)
                {
                    var attributeDetails = urlDetails.Attributes.Where(s => s.Name == "href").FirstOrDefault();
                    if (attributeDetails != null)
                    {
                        a.OwnerUrl = attributeDetails.Value;
                    }
                    a.Owner = urlDetails.InnerText;
                }
                else
                {
                    urlDetails = userDetails.SelectSingleNode("./h4[1]");
                    a.Owner = urlDetails.InnerText;
                }
                foreach (var item in items)
                {
                    var key = item.SelectSingleNode("./tr[1]/th[1]").InnerText.Trim('\n').Trim('\t');
                    var value = Regex.Replace(item.SelectSingleNode("./tr[1]/td[1]").InnerText, @"\t|\n|\r", "");

                    switch (key)
                    {
                        case "Oferit de": a.OwnBy = value; break;
                        case "Model": a.Model = value; break;
                        case "Combustibil": a.Fuel = value; break;
                        case "An de fabricatie": a.Year = value; break;
                        case "Caroserie": a.Body = value; break;
                        case "Stare": a.Condition = value; break;

                        case "Marca": a.Brand = value; break;
                        case "Culoare": a.Color = value; break;
                        case "Cutie de viteze": a.Gearbox = value; break;
                        case "Rulaj": a.Km = value; break;
                        case "Capacitate motor": a.Engine = value; break;

                        case "VIN": a.VIN = value; break;
                    }
                }

                _context.Ads.Add(a);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
