using HtmlAgilityPack;
using MedicApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
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
            var jsonClient = new WebClient();
            foreach (var item in feed.Items.Take(3)) // take the first 3 articles (for now)
            {
                var articleId = HttpUtility.ParseQueryString(item.Links[0].Uri.Query).Get("c");
                var articleJson = jsonClient.DownloadString("https://tools.cdc.gov/api/v2/resources/media/" + articleId + "?fields=contentUrl,dateModified,datePublished,sourceUrl");
                var sourceUrl = item.Links[0].Uri = new Uri(Regex.Match(articleJson, @"\""sourceUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                var contentHtml = webClient.Load(Regex.Match(articleJson, @"\""contentUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                ret += "'" + item.Title.Text + "'\n  '" + sourceUrl.ToString() + "'\n  '" + item.PublishDate + "'\n";

                if (sourceUrl.Equals("https://www.cdc.gov/coronavirus/2019-ncov/index.html"))
                {
                    // ret += "  (skipping Coronavirus page)\n"; // call ScrapeOutbreaks("https://tools.cdc.gov/api/v2/resources/media/403372.rss") instead
                    continue;
                }
                else
                {
                    var ArticleFromPage = ScrapeCDCOutbreak(contentHtml);
                    ret += "  " + ScrapeOutbreakArticle(item, contentHtml) + "\n";
                    ret += "MAIN TEXT\n";
                    ret += ScrapeCDCOutbreak(contentHtml);
                }
                    ret += "========================================================================\n";
            }
            jsonClient.Dispose();
            return ret;
        }

        public StoredArticle ScrapeOutbreakArticle(SyndicationItem item, HtmlDocument page)
        {
            var article = new StoredArticle
            {
                url = item.Links[0].Uri.ToString(),
                headline = item.Title.Text,
                main_text = "WIP",
                date_of_publication_str = item.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss")
            };
            return article;
            // use page.DocumentNode to scrape article ...
        }


        public string ScrapeCDCOutbreak(HtmlDocument webPageHtml)
        {
            var mainTextSegment = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'card-body bg-white']");
            var articleMainText = "";
            foreach(var textSegment in mainTextSegment)
            {
                articleMainText += Regex.Replace(HttpUtility.HtmlDecode(textSegment.InnerText), @"\.(?=\S)", ". ") + "\n";
            }
            return articleMainText;
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
