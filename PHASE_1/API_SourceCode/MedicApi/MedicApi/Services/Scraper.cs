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

        protected DiseaseMapper _diseaseMapper;
        protected SyndromeMapper _syndromeMapper;
        protected SymptomMapper _symptomMapper;
        protected KeyWordsMapper _keywordsMapper;
        protected LocationMapper _locationMapper;
        protected List<string> _conjunctions;

        public Scraper(DiseaseMapper diseaseMapper, SyndromeMapper syndromeMapper,
                       SymptomMapper symptomMapper, KeyWordsMapper keywordsMapper,
                       LocationMapper locationMapper, List<string> conjunctions)
        {
            this._diseaseMapper = diseaseMapper;
            this._syndromeMapper = syndromeMapper;
            this._symptomMapper = symptomMapper;
            this._keywordsMapper = keywordsMapper;
            this._locationMapper = locationMapper;
            this._conjunctions = conjunctions;
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
                        ), a)
                        { IsUpsert = true }), new BulkWriteOptions { IsOrdered = false });
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
            foreach (var item in feed.Items) // take the first 3 articles (for now)
            {
                if (!ValidArticle(item)) continue;
                var articleId = HttpUtility.ParseQueryString(item.Links[0].Uri.Query).Get("c");
                var articleJson = jsonClient.DownloadString("https://tools.cdc.gov/api/v2/resources/media/" + articleId + "?fields=contentUrl,dateModified,datePublished,sourceUrl");
                var sourceUrl = item.Links[0].Uri = new Uri(Regex.Match(articleJson, @"\""sourceUrl""\s*:\s*""([^""]*)""").Groups[1].Value);
                if (!Regex.Match(sourceUrl.ToString(), "/outbreaks/").Success) { continue; }
                var contentHtml = webClient.Load(Regex.Match(articleJson, @"\""contentUrl""\s*:\s*""([^""]*)""").Groups[1].Value);

                Console.WriteLine(item.Links[0].Uri);
                ret.Add(ScrapeArticle(item, contentHtml));
            }
            jsonClient.Dispose();
            return ret;
        }

        protected virtual StoredArticle ScrapeArticle(SyndicationItem item, HtmlDocument webPageHtml)
        {
            var sourceUrl = item.Links[0].Uri.ToString();
            var articleMainText = GetMainText(webPageHtml, sourceUrl);
            var sentences = SentencizeMainText(articleMainText);
            var locationUrl = new Uri(Regex.Replace(sourceUrl, @"([^/]+)/?$", "map.html"));
            var locations = GetLocations(locationUrl, articleMainText);
            locations = locations.Distinct().ToList();
            var reports = GenerateReportsFromMainText(sentences, locations);
            var keywords = GetKeywordsFromMainText(sentences);

            return new StoredArticle()
            {
                url = sourceUrl,
                date_of_publication_start = item.PublishDate.UtcDateTime,
                date_of_publication_end = item.PublishDate.UtcDateTime,
                date_of_publication_str = item.PublishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                headline = item.Title.Text,
                main_text = articleMainText,
                keywords = keywords,
                reports = reports,
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
                var dateRangeMatcher = @"((Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2},*\s+\d{4}),*\s* to\s*,* ((Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2},*\s+\d{4})";
                if (Regex.IsMatch(sentence, dateRangeMatcher))
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
            // if no patterened date range, manually get dates
            if (dateToAddToReport == "")
            {
                dateToAddToReport = ManuallyExtractDateFromAllSentences(ArticleSentences);
            }
            reportList.Add(CreateStoredReport(dateToAddToReport, diseasesToAddToReport, syndromesToAddToReport, symptomsToConvertToSyndromes, locationsToAdd));
            return reportList;
        }

        public List<string> GetKeywordsFromMainText(List<string> sentences)
        {
            var keywords = new List<string>();
            foreach (var sentence in sentences)
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
                    if (((!Regex.IsMatch(sentence.ToLower(), "the following groups of people") && (!Regex.IsMatch(sentence.ToLower(), "tested negative")) && (!Regex.IsMatch(sentence.ToLower(), "the disease primarily affects")))) &&
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
            var dateRangeMatcher = @"((Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2},*\s+\d{4}),*\s* to\s*,* ((Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2},*\s+\d{4})";
            var dates = Regex.Match(sentence, dateRangeMatcher);
            if (dates.Success)
            {
                var start = dates.Groups[1];
                var end = dates.Groups[14];
                return start + " - " + end;
            }
            return "";
        }

        public string ManuallyExtractDateFromAllSentences(List<string> articleSentences)
        {
            var start = "";
            var end = "";
            foreach (var sentence in articleSentences)
            {
                var dateMatcher = @"\s*(\d{1,2})?,*\s*(\d{1,2})?,*(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s*(\d{1,2})*,*\s+(\d{4})";
                var dateMatch = Regex.Match(sentence, dateMatcher);
                if (dateMatch.Success)
                {
                    try
                    {
                        var date = DateTime.Parse(dateMatch.Groups[0].Value);
                        if (start == "" || end == "")
                        {
                            start = dateMatch.Groups[0].Value;
                            end = dateMatch.Groups[0].Value;
                        }
                        else if (DateTime.Compare(DateTime.Parse(start), date) > 0)
                        {
                            start = dateMatch.Groups[0].Value;
                        }
                        else if (DateTime.Compare(DateTime.Parse(end), date) < 0)
                        {
                            end = dateMatch.Groups[0].Value;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "date could not be extracted");
                        continue;
                    }
                }
            }
            if (start == "" || end == "")
            {
                return "Date range could not be extracted.\n";
            }
            else
            {
                return start + " - " + end;
            }
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
                diseases = diseasesToAddToReport,
                syndromes = syndromesToAddToReport,
                event_date_start = dateRange.Item1,
                event_date_end = dateRange.Item2,
                event_date_str = dateStr,
                locations = locationsToAdd,
            };
        }

        public List<StoredPlace> GetLocations(Uri locationUrl, string articleMainText)
        {
            try
            {
                // test no map
                // locationUrl = new Uri("https://www.cdc.gov/listeria/outbreaks/bean-sprouts-11-14/map.html");
                var request = WebRequest.Create(locationUrl) as HttpWebRequest;
                request.Method = "HEAD";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    int statusCode = (int)response.StatusCode;
                    if (statusCode == 404)
                    {
                        return GetLocationsNoMap(articleMainText);
                    }
                    else
                    {
                        return GetLocationsFromMap(locationUrl);
                    }
                }
            }
            catch
            {
                return GetLocationsNoMap(articleMainText);
            }
        }

        public List<StoredPlace> GetLocationsNoMap(string articleMainText)
        {
            var locations = new List<StoredPlace>();
            var sentences = SentencizeMainText(articleMainText);
            foreach (var sentence in sentences)
            {
                AnalayseTextForLocations(sentence, locations);
            }
            return locations;
        }

        public List<StoredPlace> GetLocationsFromMap(Uri locationUrl)
        {
            try
            {
                var locations = new List<StoredPlace>();
                var locationWebClient = new HtmlWeb();
                var locationWebHtml = locationWebClient.Load(locationUrl);
                var locationTable = locationWebHtml.DocumentNode.SelectNodes("//*[contains(@class, 'table')]")
                                                .Nodes().Where(c => c.Name == "tbody").FirstOrDefault().ChildNodes
                                                .Where(c => c.Name == "tr");
                var locationText = SentencizeMainText(GetMainText(locationWebHtml, locationUrl.ToString()));
                if (locationTable == null)
                {
                    return GetLocationsFromMapNoTable(locationText);
                }
                else
                {
                    return GetLocationsFromMapWithTable(locationTable);
                }
            }
            catch (Exception e)
            {
                var locations = new List<StoredPlace>();
                var locationWebClient = new HtmlWeb();
                var locationWebHtml = locationWebClient.Load(locationUrl);
                var locationText = SentencizeMainText(GetMainText(locationWebHtml, locationUrl.ToString()));
                return GetLocationsFromMapNoTable(locationText);
            }
        }

        public List<StoredPlace> GetLocationsFromMapWithTable(IEnumerable<HtmlNode> locationTable)
        {
            var locations = new List<StoredPlace>();
            foreach (var location in locationTable)
            {
                //var locationString = location.ChildNodes.Where(c => c.Name == "td").FirstOrDefault().InnerText;
                var locationString = location.ChildNodes;
                foreach (var attribute in locationString)
                {
                    locations.AddRange(_locationMapper.ExtractLocations(attribute.InnerText));
                }
            }
            return locations;
        }

        public List<StoredPlace> GetLocationsFromMapNoTable(List<string> locationText)
        {
            var locations = new List<StoredPlace>();
            // get text from map page to analyse for locations
            foreach (var sentence in locationText)
            {
                AnalayseTextForLocations(sentence, locations);
            }
            return locations;
        }

        public void AnalayseTextForLocations(string sentence, List<StoredPlace> locations)
        {
            foreach (var match in Regex.Matches(sentence, @"([A-Z][\w-]*(\s+[A-Z][\w-]*)*)"))
            {
                var locationCheck = match.ToString();
                locations.AddRange(_locationMapper.ExtractLocations(locationCheck));
            }
        }

        public string GetMainText(HtmlDocument webPageHtml, string sourceUrl)
        {
            try
            {
                // test no map
                var symptomsAndSyndromesUrl = new Uri(Regex.Replace(sourceUrl, @"([^/]+)/?$", "signs-symptoms.html"));

                var request = WebRequest.Create(symptomsAndSyndromesUrl) as HttpWebRequest;
                request.Method = "HEAD";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    int statusCode = (int)response.StatusCode;
                    if (statusCode == 404)
                    {
                        return OnlyExtractMainText(webPageHtml);
                    }
                    else
                    {
                        var mainText1 = OnlyExtractMainText(webPageHtml);
                        var symptomsAndSyndromesClient = new HtmlWeb();
                        var symptomsAndSyndromesWebHtml = symptomsAndSyndromesClient.Load(symptomsAndSyndromesUrl);
                        var symptomsAndSyndromes = OnlyExtractMainText(symptomsAndSyndromesWebHtml);
                        if (mainText1.Equals(symptomsAndSyndromes))
                        {
                            symptomsAndSyndromes = "\n";
                        }
                        return mainText1 + "\n" + symptomsAndSyndromes + "\n";
                    }
                }
            }
            catch
            {
                return OnlyExtractMainText(webPageHtml);
            }
        }

        public string OnlyExtractMainText(HtmlDocument webPageHtml)
        {
            var mainTextSegment = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'syndicate']");
            var articleMainText = "";
            if (mainTextSegment != null)
            {
                foreach (var textSegment in mainTextSegment)
                {
                    string pattern = @"([^\w]*external icon[^\w]*)+|[|\\^&\r\n]+";
                    Regex rgx = new Regex(pattern);
                    var uncleanText = Regex.Replace(HttpUtility.HtmlDecode(textSegment.InnerText), @"\.(?=\S)", ". ");
                    articleMainText += rgx.Replace(uncleanText, " ") + "\n\n";
                }
            }
            else
            {
                mainTextSegment = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'col-md-12']");
                if (mainTextSegment != null)
                {
                    foreach (var textSegment in mainTextSegment)
                    {
                        string pattern = @"([^\w]*external icon[^\w]*)+|[|\\^&\r\n]+";
                        Regex rgx = new Regex(pattern);
                        var uncleanText = Regex.Replace(HttpUtility.HtmlDecode(textSegment.InnerText), @"\.(?=\S)", ". ");
                        articleMainText += rgx.Replace(uncleanText, " ") + "\n\n";
                    }
                }
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

