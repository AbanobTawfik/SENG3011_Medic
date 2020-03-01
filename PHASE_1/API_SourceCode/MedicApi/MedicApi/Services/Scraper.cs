using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MedicApi.Services
{
    public class Scraper
    {
        public ObservableCollection<Object> ScrapedData { get; set; }

        public List<String> ScrapeData(string url)
        {
            var webClient = new HtmlWeb();
            var webPageHtml = webClient.Load(url);

            var outbreaks = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'feed-item-title']");

            List<String> ret = new List<String>();
            
            foreach (var outbreak in outbreaks)
            {
                var outbreakName = HttpUtility.HtmlDecode(outbreak.InnerText);
                var linkToArticle = outbreak.Attributes.Where(attribute => attribute.Name == "href").FirstOrDefault().DeEntitizeValue;
                ret.Add(linkToArticle);
            }

            return ret;
        }
    }
}
