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
            rssUrl = "https://tools.cdc.gov/api/v2/resources/media/132608.rss?max=30";
        }

        protected override bool ValidArticle(SyndicationItem item)
        {
            return item.Categories.Any(c => c.Name == "Media Statement");
        }

        protected override StoredArticle ScrapeArticle(SyndicationItem item, HtmlDocument webPageHtml)
        {
            var mainText = GetMainText(webPageHtml);
            if (!mainText.Contains("case", StringComparison.OrdinalIgnoreCase)) // No reports, skip article
                return null;

            List<string> diseases = new List<string>();
            AnalyseSentenceForKeyWords(mainText, _diseaseMapper, diseases, false);
            if (diseases.Count == 0) // No diseases, skip article
                return null;
            List<string> syndromes = new List<string>(), symptoms = new List<string>();
            AnalyseSentenceForKeyWords(mainText, _syndromeMapper, syndromes, false);
            AnalyseSentenceForKeyWords(mainText, _symptomMapper, symptoms, true);
            AddSyndromesFromSymptoms(syndromes, symptoms);
            List<StoredPlace> locations = new List<StoredPlace>();
            AnalayseTextForLocations(mainText, locations);
            
            var dateRange = ManuallyExtractDateFromAllSentences(mainText.Split(new char[] { '.', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            if (dateRange == "Date range could not be extracted.\n")
            {
                // if (mainText.Contains("today", StringComparison.OrdinalIgnoreCase))
                    dateRange = item.PublishDate.ToString("MMMM dd yyyy") + " " +
                                item.PublishDate.ToString("MMMM dd yyyy");
            }

            var reports = new List<StoredReport> {
                CreateStoredReport(dateRange, diseases, syndromes, symptoms, locations.Distinct().ToList())
            };
            reports[0].syndromes.Clear();
            var keywords = new List<string>();
            AnalyseSentenceForKeyWords(mainText, _keywordsMapper, keywords, true);

            return new StoredArticle()
            {
                url = item.Links[0].Uri.ToString(),
                date_of_publication_start = item.PublishDate.UtcDateTime,
                date_of_publication_end = item.LastUpdatedTime.UtcDateTime,
                date_of_publication_str = item.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                headline = item.Title.Text,
                main_text = mainText,
                keywords = keywords,
                reports = reports,
            };
        }

        private string GetMainText(HtmlDocument webPageHtml)
        {
            var bodySegments = webPageHtml.DocumentNode.SelectNodes("//div[contains(@class, 'card-body')]");
            var fullText = "";
            if (bodySegments == null) return fullText;
            foreach (var bodySegment in bodySegments)
            {   // Remove unneccessary segments
                if (bodySegment.InnerText.Substring(0, 22) != "For Immediate Release:" &&
                    bodySegment.InnerText.Substring(0, 3) != "###")
                {   // Build fullText from text of children
                    foreach (var child in bodySegment.ChildNodes)
                    {   // Build list from unordered list
                        if (child.Name == "ul")
                        {
                            foreach (var item in child.ChildNodes)
                            {
                                if (item.Name != "li") continue;
                                fullText += "  - " + item.InnerText + "\n";
                            }
                        }
                        else
                            fullText += child.InnerText + "\n";
                    }
                }
            }
            return fullText;
        }
    }
}
