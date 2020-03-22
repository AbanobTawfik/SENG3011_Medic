using HtmlAgilityPack;
using MedicApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class NewsroomScraper : Scraper
    {
        public NewsroomScraper(DiseaseMapper diseaseMapper, SyndromeMapper syndromeMapper,
                               SymptomMapper symptomMapper, KeyWordsMapper keywordsMapper,
                               LocationMapper locationMapper, List<string> conjunctions) :
                               base(diseaseMapper, syndromeMapper, symptomMapper,
                                   keywordsMapper, locationMapper, conjunctions)
        {
            rssUrl = "https://tools.cdc.gov/api/v2/resources/media/132608.rss?max=10";
        }

        protected override bool ValidArticle(SyndicationItem item)
        {
            return item.Categories.Any(c => c.Name == "Media Statement");
        }

        protected override StoredArticle ScrapeArticle(SyndicationItem item, HtmlDocument webPageHtml)
        {
            var article = new StoredArticle
            {
                url = item.Links[0].Uri.ToString(),
                headline = item.Title.Text,
                main_text = "WIP",
                date_of_publication_str = item.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss")
            };
            return article;
        }
    }
}
