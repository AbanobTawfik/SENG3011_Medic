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

using MongoDB.Bson;
using MongoDB.Driver;

namespace MedicApi.Services
{
    public class Scraper
    {
        static readonly MongoClient client = new MongoClient("mongodb+srv://medics:adfrZUBj4IF4TNibOnLxQKansolSPoW6@cluster0-nqmfu.mongodb.net/test?retryWrites=true&w=majority");
        static readonly bool storeArticles = true; // debugging

        protected string rssUrl = "https://tools.cdc.gov/api/v2/resources/media/285676.rss";

        private DiseaseMapper  _diseaseMapper;
        private SyndromeMapper _syndromeMapper;
        private SymptomMapper  _symptomMapper;
        private KeyWordsMapper _keywordsMapper;
        private LocationMapper _locationMapper;
        private List<string>   _conjunctions;

        public Scraper(DiseaseMapper diseaseMapper, SyndromeMapper syndromeMapper,
                       SymptomMapper symptomMapper, KeyWordsMapper keywordsMapper,
                       LocationMapper locationMapper, List<string> conjunctions)
        {
            this._diseaseMapper  = diseaseMapper;
            this._syndromeMapper = syndromeMapper;
            this._symptomMapper  = symptomMapper;
            this._keywordsMapper = keywordsMapper;
            this._locationMapper = locationMapper;
            this._conjunctions   = conjunctions;
        }

        public async void ScrapeAndStoreOutbreaksFromRSS(string url)
        {
            try
            {
                var db = client.GetDatabase("articles");
                var collections = db.GetCollection<StoredArticle>("articles");
                List<StoredArticle> articles = ScrapeRSS(url);

                if (storeArticles)
                {
                    await collections.BulkWriteAsync(articles.Select(a =>
                        new ReplaceOneModel<StoredArticle>(Builders<StoredArticle>.Filter.And(
                            Builders<StoredArticle>.Filter.Eq("_id", a.url),
                            Builders<StoredArticle>.Filter.Lt("date_of_publication_end", a.date_of_publication_end)
                        ), a) { IsUpsert = true }), new BulkWriteOptions { IsOrdered = false });
                }
            }
            catch (MongoBulkWriteException e)
            {
                var bulkWriteErrors = e.WriteErrors;
                foreach (BulkWriteError bulkWriteError in bulkWriteErrors)
                {   // Ignore duplicate key exception
                    if (bulkWriteError.Category != ServerErrorCategory.DuplicateKey)
                        Console.WriteLine("Exception while writing record: " + bulkWriteError.Message);
                };
            }
            catch (Exception e)
            {
                Console.Write("EXCEPTION:\n" + e.Message);
            }
        }

        /*
         * Returns a list of Articles from a given RSS feed url.
         * Currently returns a string for debugging.
         */
        public List<StoredArticle> ScrapeRSS(string url)
        {
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            var ret = new List<StoredArticle>();
            var webClient = new HtmlWeb();
            var jsonClient = new WebClient();
            foreach (var item in feed.Items.Take(3)) // take the first 3 articles (for now)
            {
                if (!ValidArticle(item)) continue;
                var articleId = HttpUtility.ParseQueryString(item.Links[0].Uri.Query).Get("c");
                var articleJson = jsonClient.DownloadString("https://tools.cdc.gov/api/v2/resources/media/" + articleId + "?fields=contentUrl,dateModified,datePublished,sourceUrl");
                var sourceUrl = item.Links[0].Uri = new Uri(Regex.Match(articleJson, @"\""sourceUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                var contentHtml = webClient.Load(Regex.Match(articleJson, @"\""contentUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                
                Console.WriteLine(item.Links[0].Uri);
                ret.Add(ScrapeArticle(item, contentHtml));
            }
            jsonClient.Dispose();
            return ret;
        }

        protected virtual StoredArticle ScrapeArticle(SyndicationItem item, HtmlDocument webPageHtml)
        {
            var sourceUrl       = item.Links[0].Uri.ToString();
            var articleMainText = GetMainText(webPageHtml);
            var sentences       = SentencizeMainText(articleMainText);
            var locationUrl     = new Uri(Regex.Replace(sourceUrl, @"/index.html*$", "/map.html"));
            var locations       = GetLocationsFromMap(locationUrl);
            var reports         = GenerateReportsFromMainText(sentences, locations);
            var keywords        = GetKeywordsFromMainText(sentences);

            return new StoredArticle()
            {
                url = sourceUrl,
                date_of_publication_start = item.PublishDate.UtcDateTime,
                date_of_publication_end   = item.PublishDate.UtcDateTime,
                date_of_publication_str   = item.PublishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                headline                  = item.Title.Text,
                main_text                 = articleMainText,
                keywords                  = keywords,
                reports                   = reports,
            };
        }

        public List<StoredReport> GenerateReportsFromMainText(List<String> ArticleSentences, List<StoredPlace> locationsToAdd)
        {

            var reportList = new List<StoredReport>();
            var diseasesToAddToReport = new List<string>();
            var syndromesToAddToReport = new List<string>();
            var symptomsToConvertToSyndromes = new List<string>();
            var dateToAddToReport = "";
            foreach (var sentence in ArticleSentences)
            {
                // if there is a conjunction and a new disease, we want to make a new report for it and reset our running paramters
                if (HasConjunction(sentence, diseasesToAddToReport))
                {
                    reportList.Add(CreateStoredReport(dateToAddToReport, diseasesToAddToReport, syndromesToAddToReport, symptomsToConvertToSyndromes, locationsToAdd));
                    ResetLists(out diseasesToAddToReport, out syndromesToAddToReport, out symptomsToConvertToSyndromes);
                }
                // if we get a date range phrase, extract date
                if (Regex.IsMatch(sentence, @"Illnesses started on dates ranging from .* to .*\."))
                {
                    dateToAddToReport = GetDateRangeFromText(sentence);
                }
                // extract the diseases from the sentence
                // Console.WriteLine(sentence);
                AnalyseSentenceForKeyWords(sentence, _diseaseMapper, diseasesToAddToReport, false);
                AnalyseSentenceForKeyWords(sentence, _syndromeMapper, syndromesToAddToReport, false);
                AnalyseSentenceForKeyWords(sentence, _symptomMapper, symptomsToConvertToSyndromes, true);
                // add all symptoms

            }
            // add the report
            reportList.Add(CreateStoredReport(dateToAddToReport, diseasesToAddToReport, syndromesToAddToReport, symptomsToConvertToSyndromes, locationsToAdd));
            return reportList;
        }

        public List<string> GetKeywordsFromMainText(List<string> sentences)
        {
            var keywords = new List<string>();
            foreach(var sentence in sentences)
            {
                AnalyseSentenceForKeyWords(sentence, _keywordsMapper, keywords, true);
            }
            return keywords;
        }

        private void AnalyseSentenceForKeyWords(string sentence, Mapper mapper, List<string> list, bool storeOriginal)
        {
            var keyWordList = mapper.AllReferences();
            if (!storeOriginal)
            {
                foreach (var keyWord in keyWordList)
                {
                    var keyWordToAdd = mapper.GetCommonKeyName(keyWord);
                    if (((!Regex.IsMatch(sentence.ToLower(), "the following groups of people") || (!Regex.IsMatch(sentence.ToLower(), "tested negative")))) && 
                        (Regex.IsMatch(sentence.ToLower(), @"\b" + keyWord.ToLower() + @"\b") && !list.Contains(keyWordToAdd, StringComparer.OrdinalIgnoreCase)))
                    {
                        list.Add(keyWordToAdd);
                    }
                }
            }
            else
            {
                foreach (var keyWord in keyWordList)
                {
                    if (Regex.IsMatch(sentence.ToLower(), @"\b" + keyWord.ToLower() + @"\b") && !list.Contains(keyWord, StringComparer.OrdinalIgnoreCase))
                    {
                        list.Add(keyWord);
                    }
                }
            }
        }

        public string GetDateRangeFromText(string sentence)
        {
            var dates = Regex.Match(sentence, @"Illnesses started on dates ranging from (.*) to (.*)\.");
            if (dates.Success)
            {
                var start = dates.Groups[1];
                var end   = dates.Groups[2];
                return start + " - " + end;
            }
            return "";
        }

        public void ResetLists(out List<string> diseasesToAddToReport,
                               out List<string> syndromesToAddToReport,
                               out List<string> symptomsToConvertToSyndromes)
        {
            diseasesToAddToReport = new List<string>();
            syndromesToAddToReport = new List<string>();
            symptomsToConvertToSyndromes = new List<string>();
        }

        public void AddSyndromesFromSymptoms(List<string> syndromes, List<string> symptoms)
        {
            var syndromesToAdd = _symptomMapper.HighestRank(symptoms);
            /*Console.WriteLine("Symptoms in Article:");
            foreach (var symptom in symptoms)
            {
                Console.WriteLine(symptom);
            }*/

            foreach (var syndrome in syndromesToAdd)
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

        public StoredReport CreateStoredReport(string dateToAddToReport,
                                               List<string> diseasesToAddToReport,
                                               List<string> syndromesToAddToReport,
                                               List<string> symptomsToConvertToSyndromes,
                                               List<StoredPlace> locationsToAdd)
        {
            var dateStr = DateParser.ParseDateStr(dateToAddToReport);
            var dateRange = DateUtils.DateStrToDateRange(dateStr);

            AddSyndromesFromSymptoms(syndromesToAddToReport, symptomsToConvertToSyndromes);
            return new StoredReport
            {
                diseases         = diseasesToAddToReport,
                syndromes        = syndromesToAddToReport,
                event_date_start = dateRange.Item1,
                event_date_end   = dateRange.Item2,
                event_date_str   = dateStr,
                locations        = locationsToAdd,
            };
        }

        public List<StoredPlace> GetLocationsFromMap(Uri locationUrl)
        {
            var locations = new List<StoredPlace>();
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
                    var place = new StoredPlace()
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

        protected virtual bool ValidArticle(SyndicationItem item)
        {
            return item.Links[0].Uri.Query != "?m=285676&c=403352"; // skip coronavirus page
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
