using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WebCrawler.Console;
using WebCrawler.Database.Models;

namespace WebCrawler.Database
{
    public class AutovitAds : Ads
    {
        private DatabaseContext _context;

        public AutovitAds(DatabaseContext context)
        {
            _context = context;
        }

        public void ProcessTable()
        {
            int counter = 0;
            string link, id;
            try
            {
                for (var page = 1; page <= 1000; page++)
                {
                    System.Console.WriteLine("Page " + page);
                    var url = "https://www.autovit.ro/autoturisme/?page=1" + page;
                    var html = GetHtml(url, true);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    var carTable = htmlDocument.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("class", "") == "offers list");

                    var rows = carTable.FirstOrDefault().Descendants("article");

                    foreach (var row in rows)
                    {
                        try
                        {
                            link = row.GetAttributeValue("data-href", "");
                            id = row.GetAttributeValue("data-ad-id", "");
                            link = link.Split("#")[0];
                            System.Console.WriteLine(link);

                            if (_context.Ads.Any(s => s.Url == link) || _context.Ads.Any(s => s.UniqueIdentifier == id))
                            {
                                counter++;
                                System.Console.WriteLine("Found");
                                if (counter == 100)
                                {
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                counter = 0;
                            }

                            ProcessAd(link, id);
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

        public void ProcessAd(string link, string id)
        {
            var carHtml = GetHtml(link);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(carHtml);
            
            Ad a = new Ad();
            a.ModifiedDate = a.CreatedDate = DateTime.UtcNow;
            a.UniqueIdentifier = id;
            a.Url = link;
            var description = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("id", "").Contains("description")).FirstOrDefault();
            a.Description = description != null ? description.InnerText : "";

            var address = htmlDoc.DocumentNode.Descendants("span").Where(s => s.GetAttributeValue("class", "").Contains("seller-box__seller-address__label")).FirstOrDefault();
            a.Address = address != null ? address.InnerText.TrimStart().TrimEnd() : "";
            var photos = htmlDoc.DocumentNode.Descendants("img").Where(s => s.GetAttributeValue("class", "").Contains("bigImage"));
            a.PhotoUrl = photos.FirstOrDefault() != null ? photos.FirstOrDefault().GetAttributeValue("data-lazy", "") : "";

            //int count = 1;
            //foreach (var photo in photos)
            //{
            //    var url = photo.GetAttributeValue("data-lazy", "");
            //    var url1 = url.Split("image;s=");
            //    var url2 = url.Split(";cars_");
            //    url = url1[0] + "image;s=720x480;cars_" + url2[1];
            //    this.SavePicture(url, Path.Combine(this._basePath + "Autovit", a.UniqueIdentifier + "_" + count + Path.GetExtension(url)));
            //    count++;
            //}

            var idstring = link.Remove(0, link.IndexOf("-ID") + 3);
            idstring = idstring.Remove(idstring.IndexOf("."), idstring.Length - idstring.IndexOf("."));
            var resp1 = GetHtml("https://www.autovit.ro/ajax/misc/contact/multi_phone/" + idstring + "/0/");
            resp1 = resp1.Replace("{\"value\":\"", "").Replace("\"}", "");
            a.Phone1 = resp1;
            System.Console.WriteLine(resp1);

            var price = htmlDoc.DocumentNode.Descendants("span").Where(s => s.GetClasses().Contains("offer-price__number")).FirstOrDefault();
            a.Price = price != null ? Regex.Replace(price.InnerText, @"\t|\n|\r|'|;", "") : "";

            var div = htmlDoc.DocumentNode.Descendants("div").Where(s => s.GetAttributeValue("id", "").Contains("parameters")).FirstOrDefault();
            var items = div.Descendants("li").Where(s => s.GetClasses().Contains("offer-params__item"));

            foreach (var item in items)
            {
                string value = "";
                var key = item.SelectSingleNode("./span[1]").InnerText.Trim('\n').Trim('\t');
                var val = item.SelectSingleNode("./div[1]").InnerHtml;
                if (val.Contains("<a"))
                {
                    value = Regex.Replace(item.SelectSingleNode("./div[1]/a[1]").InnerText, @"\t|\n|\r", "").TrimStart().TrimEnd();
                }
                else
                {
                    value = Regex.Replace(item.SelectSingleNode("./div[1]").InnerText, @"\t|\n|\r", "").TrimStart().TrimEnd();
                }
                switch (key)
                {
                    case "Oferit de": a.OwnBy = value; break;
                    case "Marca": a.Brand = value; break;
                    case "Model": a.Model = value; break;
                    case "Anul fabricatiei": a.Year = value; break;
                    case "Km": a.Km = value; break;
                    case "VIN": a.VIN = value; break;
                    case "Capacitate cilindrica": a.Engine = value; break;
                    case "Combustibil": a.Fuel = value; break;
                    case "Putere": a.Power = value; break;
                    case "Cutie de viteze": a.Gearbox = value; break;
                    case "Transmisie": a.Transmission = value; break;
                    case "Norma de poluare": a.Emissions = value; break;
                    case "Emisii CO2": a.CO2 = value; break;
                    case "Caroserie": a.Body = value; break;
                    case "Numar de portiere": a.Doors = value; break;
                    case "Culoare": a.Color = value; break;
                    case "Tara de origine": a.Country = value; break;
                    case "Primul proprietar": a.First = value; break;
                    case "Fara accident in istoric": a.Condition = value; break;
                }
            }

            //_context.Ads.Add(a);
            //_context.SaveChanges();
        }
    }
}
