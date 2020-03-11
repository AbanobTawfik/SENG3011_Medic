using HtmlAgilityPack;
using MedicApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

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

            var internationalOutbreaks = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'bullet-list feed-item-list']").Nodes().Where(n => n.HasChildNodes);

            foreach(var outbreak in internationalOutbreaks)
            {
                var outbreakName = HttpUtility.HtmlDecode(outbreak.InnerText);
                var linkToArticle = outbreak.FirstChild.Attributes.Where(attribute => attribute.Name == "href").FirstOrDefault().DeEntitizeValue;
                ret.Add(linkToArticle);
            }

            return ret;
        }

        /*
         * Returns a list of Articles from a given RSS feed url.
         * Currently returns a string for debugging.
         */
        public string ScrapeOutbreaksRSS(string url)
        {
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            var ret = "Scraping '" + url + "':\n\n";
            var webClient = new HtmlWeb();
            foreach (var item in feed.Items.Take(3)) // take the first 3 articles (for now)
            {
                var webPageHtml = webClient.Load(item.Links[0].Uri);
                var uri = item.Links[0].Uri = webClient.ResponseUri;
                ret += "'" + item.Title.Text + "'\n  '" + uri.ToString() + "'\n  '" + item.PublishDate + "'\n";

                if (uri.Equals("https://www.cdc.gov/coronavirus/2019-ncov/index.html"))
                    ret += "  (skipping Coronavirus page)\n"; // call ScrapeOutbreaks("https://tools.cdc.gov/api/v2/resources/media/403372.rss") instead
                else
                    ret += "  " + ScrapeOutbreakArticle(item, webPageHtml) + "\n";
                ret += "\n";
            }
            return ret;
        }

        public Article ScrapeOutbreakArticle(SyndicationItem item, HtmlDocument page)
        {
            var article = new Article
            {
                url = item.Links[0].Uri.ToString(),
                headline = item.Title.Text,
                main_text = "WIP",
                date_of_publication = item.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss")
            };
            return article;
        }

        public Article ScrapeUrl(string url)
        {
            return null;
        }

        public Article ScrapeCDCOutbreak(string url)
        {
            return null;
        }

        public Article ScrapeCDCBasicInformation(string url)
        {
            return null;
        }

        public Article ScrapeCDCExposure(string url)
        {
            return null;
        }

        public Article ScrapeCDCTravelNoticeAlert(string url)
        {
            return null;
        }

        public Article ScrapeCDCTravelNoticeWatch(string url)
        {
            return null;
        }

        public Article ScrapeCDCTravelNoticeWarning(string url)
        {
            return null;
        }
    }
}
