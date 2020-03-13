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
        private DiseaseMapper _diseaseMapper;
        private SyndromeMapper _syndromeMapper;

        public Scraper(DiseaseMapper diseaseMapper, SyndromeMapper syndromeMapper)
        {
            this._diseaseMapper = diseaseMapper;
            this._syndromeMapper = syndromeMapper;
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
                    ret += "  " + ScrapeOutbreakArticle(item, contentHtml) + "\n";
                    ret += "MAIN TEXT\n";
                    var locationPageUrl = new Uri(Regex.Replace(sourceUrl.ToString(), @"/index.html*$", "/map.html"));
                    ret += ScrapeCDCOutbreak(locationPageUrl, contentHtml);
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


        public string ScrapeCDCOutbreak(Uri locationUrl, HtmlDocument webPageHtml)
        {
            // scrape main text
            var mainTextSegment = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'card-body bg-white']");
            var articleMainText = "";
            foreach (var textSegment in mainTextSegment)
            {
                string pattern = @"([^\w]*external icon[^\w]*)+|[|\\^&\r\n]+";
                Regex rgx = new Regex(pattern);
                var uncleanText = Regex.Replace(HttpUtility.HtmlDecode(textSegment.InnerText), @"\.(?=\S)", ". ");
                articleMainText += rgx.Replace(uncleanText, " ") + "\n\n";
                var cleanText = rgx.Replace(uncleanText, " ");
            }
            // scrape locations
            var locations = new List<string>();
            var locationWebClient = new HtmlWeb();
            var locationWebHtml = locationWebClient.Load(locationUrl);
            var locationTable = locationWebHtml.DocumentNode.SelectNodes("//*[@class = 'table table-bordered table-striped']")
                                                            .Nodes().Where(c => c.Name == "tbody").FirstOrDefault().ChildNodes
                                                            .Where(c => c.Name == "tr");
            foreach (var location in locationTable)
            {
                var locationString = location.ChildNodes.Where(c => c.Name == "td").FirstOrDefault().InnerText;
                if (!locationString.ToLower().Equals("total"))
                {
                    locations.Add(locationString);
                }
            }
            var x = _diseaseMapper.GetCommonKeyName("coronavirus");
            // scrape diseases

            // scrape symptoms

            // scrape date of incident
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

//public List<String> ScrapeData(string url)
//{
//    var webClient = new HtmlWeb();
//    var webPageHtml = webClient.Load(url);

//    var outbreaks = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'feed-item-title']");

//    List<String> ret = new List<String>();

//    foreach (var outbreak in outbreaks)
//    {
//        var outbreakName = HttpUtility.HtmlDecode(outbreak.InnerText);
//        var linkToArticle = outbreak.Attributes.Where(attribute => attribute.Name == "href").FirstOrDefault().DeEntitizeValue;
//        ret.Add(linkToArticle);
//    }

//    var internationalOutbreaks = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'bullet-list feed-item-list']").Nodes().Where(n => n.HasChildNodes);

//    foreach(var outbreak in internationalOutbreaks)
//    {
//        var outbreakName = HttpUtility.HtmlDecode(outbreak.InnerText);
//        var linkToArticle = outbreak.FirstChild.Attributes.Where(attribute => attribute.Name == "href").FirstOrDefault().DeEntitizeValue;
//        ret.Add(linkToArticle);
//    }

//    return ret;
//}
