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
        private SymptomMapper _symptomMapper;
        private List<string> _conjunctions;

        public Scraper(DiseaseMapper diseaseMapper, SyndromeMapper syndromeMapper, SymptomMapper symptomMapper, List<string> conjunctions)
        {
            this._diseaseMapper = diseaseMapper;
            this._syndromeMapper = syndromeMapper;
            this._symptomMapper = symptomMapper;
            this._conjunctions = conjunctions;
        }

        /*
         * Returns a list of Articles from a given RSS feed url.
         * Currently returns a string for debugging.
         */
        public List<Article> ScrapeOutbreaksRSS(string url)
        {
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            var ret = new List<Article>();
            var webClient = new HtmlWeb();
            var jsonClient = new WebClient();
            foreach (var item in feed.Items.Take(3)) // take the first 3 articles (for now)
            {
                var articleId = HttpUtility.ParseQueryString(item.Links[0].Uri.Query).Get("c");
                var articleJson = jsonClient.DownloadString("https://tools.cdc.gov/api/v2/resources/media/" + articleId + "?fields=contentUrl,dateModified,datePublished,sourceUrl");
                var sourceUrl = item.Links[0].Uri = new Uri(Regex.Match(articleJson, @"\""sourceUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                var contentHtml = webClient.Load(Regex.Match(articleJson, @"\""contentUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                if (sourceUrl.Equals("https://www.cdc.gov/coronavirus/2019-ncov/index.html"))
                {
                    // ret += "  (skipping Coronavirus page)\n"; // call ScrapeOutbreaks("https://tools.cdc.gov/api/v2/resources/media/403372.rss") instead
                    continue;
                }
                else
                {
                    var locationPageUrl = new Uri(Regex.Replace(sourceUrl.ToString(), @"/index.html*$", "/map.html"));
                    ret.Add(ScrapeCDCOutbreak(item, contentHtml));
                }
            }
            jsonClient.Dispose();
            return ret;
        }

        public Article ScrapeCDCOutbreak(SyndicationItem item, HtmlDocument webPageHtml)
        {
            var sourceUrl = item.Links[0].Uri.ToString();
            var articleMainText = GetMainText(webPageHtml);
            var sentences = SentencizeMainText(articleMainText);
            var locationUrl = new Uri(Regex.Replace(sourceUrl, @"/index.html*$", "/map.html"));
            var locations = GetLocationsFromMap(locationUrl);
            var reports = GenerateReportsFromMainText(sentences, locations);
            return new Article()
            {
                url = sourceUrl,
                headline = item.Title.Text,
                main_text = articleMainText,
                date_of_publication = item.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                reports = reports

            };
        }

        public List<Report> GenerateReportsFromMainText(List<String> ArticleSentences, List<Place> locationsToAdd)
        {
            // we want to now analyse each sentence for 
            // 1. date range
            // 2. syndromes/diseases
            // 3. conjunction terms (furthermore/however/also etc.)
            var reportList = new List<Report>();
            var reportToAdd = new Report();
            var diseasesToAddToReport = new List<string>();
            var syndromesToAddToReport = new List<string>();
            var symptomsToConvertToSyndromes = new List<string>();
            var dateToAddToReport = "";
            foreach (var sentence in ArticleSentences)
            {
                if (HasConjunction(sentence, diseasesToAddToReport) && reportList.Count > 1)
                {
                    // add the features to the report
                    reportToAdd.event_date = dateToAddToReport;
                    reportToAdd.diseases = diseasesToAddToReport;
                    reportToAdd.locations = locationsToAdd;
                    // add new symptoms from syndromes
                    AddSyndromesFromSymptoms(syndromesToAddToReport, symptomsToConvertToSyndromes);
                    reportToAdd.syndromes = syndromesToAddToReport;
                    // add the report
                    reportList.Add(reportToAdd);
                    // make a new report object
                    reportToAdd = new Report();
                    diseasesToAddToReport = new List<string>();
                    syndromesToAddToReport = new List<string>();
                    symptomsToConvertToSyndromes = new List<string>();
                }
                if (Regex.IsMatch(sentence, @"Illnesses started on dates ranging from .* to .*\."))
                {
                    var dates = Regex.Match(sentence, @"Illnesses started on dates ranging from (.*) to (.*)\.");
                    if (dates.Success)
                    {
                        var start = dates.Groups[1];
                        var end = dates.Groups[2];
                        dateToAddToReport = start + " - " + end;
                    }
                }
                var allDiseases = _diseaseMapper.AllReferences();
                var allSyndromes = _syndromeMapper.AllReferences();
                var allSymptoms = _symptomMapper.AllReferences();
                foreach(var disease in allDiseases)
                {
                    var diseaseToAdd = _diseaseMapper.GetCommonKeyName(disease);
                    if (Regex.IsMatch(sentence.ToLower(), " " + disease + " ") && !diseasesToAddToReport.Contains(diseaseToAdd))
                    {
                        diseasesToAddToReport.Add(diseaseToAdd);
                    }
                }
                // first initial check for each syndrome
                foreach (var syndrome in allSyndromes)
                {
                    var syndromeToAdd = _syndromeMapper.GetCommonKeyName(syndrome);
                    if (Regex.IsMatch(sentence.ToLower(), " " + syndromeToAdd + " ") && !syndromesToAddToReport.Contains(syndromeToAdd))
                    {
                        syndromesToAddToReport.Add(syndromeToAdd);
                    }
                }
                // add all symptoms
                foreach(var symptom in allSymptoms)
                {
                    if (Regex.IsMatch(sentence.ToLower(), " " + symptom) && !symptomsToConvertToSyndromes.Contains(symptom))
                    {
                        symptomsToConvertToSyndromes.Add(symptom);
                    }
                }
            }
            AddSyndromesFromSymptoms(syndromesToAddToReport, symptomsToConvertToSyndromes);
            reportToAdd.event_date = dateToAddToReport;
            reportToAdd.diseases = diseasesToAddToReport;
            reportToAdd.locations = locationsToAdd;
            reportToAdd.syndromes = syndromesToAddToReport;
            // add the report
            reportList.Add(reportToAdd);
            return reportList;
        }

        public void AddSyndromesFromSymptoms(List<string> syndromes, List<string> symptoms)
        {
            var syndromesToAdd = _symptomMapper.HighestRank(symptoms);
            foreach(var syndrome in syndromesToAdd)
            {
                if (!syndromes.Contains(syndrome))
                {
                    syndromes.Add(syndrome);
                }
            }
        }

        public bool HasConjunction(string sentence, List<string> diseases)
        {
            var hasConjunction = false;
            var oldDisease = true;
            foreach (var conjunction in this._conjunctions)
            {
                if (sentence.Contains(conjunction))
                {
                    hasConjunction = true;
                }
            }
            var allDiseases = _diseaseMapper.AllReferences();
            foreach (var disease in allDiseases)
            {
                var diseaseToAdd = _diseaseMapper.GetCommonKeyName(disease);
                if (Regex.IsMatch(sentence.ToLower(), " " + disease + " ") && !diseases.Contains(diseaseToAdd))
                {
                    oldDisease = false;
                }
            }
            return hasConjunction && !oldDisease;
        }

        public List<Place> GetLocationsFromMap(Uri locationUrl)
        {
            var locations = new List<Place>();
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
                    var place = new Place()
                    {
                        country = locationString,
                        location = locationString
                    };
                    locations.Add(place);
                }
            }
            return locations;
        }

        public string GetMainText(HtmlDocument webPageHtml)
        {
            var mainTextSegment = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'card-body bg-white']");
            var articleMainText = "";
            foreach (var textSegment in mainTextSegment)
            {
                string pattern = @"([^\w]*external icon[^\w]*)+|[|\\^&\r\n]+";
                Regex rgx = new Regex(pattern);
                var uncleanText = Regex.Replace(HttpUtility.HtmlDecode(textSegment.InnerText), @"\.(?=\S)", ". ");
                articleMainText += rgx.Replace(uncleanText, " ") + "\n\n";
            }
            return articleMainText;
        }

        public List<string> SentencizeMainText(string mainText)
        {
            mainText = Regex.Replace(mainText, @"E\. coli", "ecolidisease");
            var sentences = new List<string>();
            string[] sectionSentences = Regex.Split(mainText, @"(?<=[\.!\?])\s+");
            foreach (var sentence in sectionSentences)
            {
                sentences.Add(sentence);
            }
            return sentences;
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
