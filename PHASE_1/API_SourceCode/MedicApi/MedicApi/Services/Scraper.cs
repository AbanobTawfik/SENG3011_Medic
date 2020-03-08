﻿using HtmlAgilityPack;
using MedicApi.Models;
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

            var internationalOutbreaks = webPageHtml.DocumentNode.SelectNodes("//*[@class = 'bullet-list feed-item-list']").Nodes().Where(n => n.HasChildNodes);

            foreach(var outbreak in internationalOutbreaks)
            {
                var outbreakName = HttpUtility.HtmlDecode(outbreak.InnerText);
                var linkToArticle = outbreak.FirstChild.Attributes.Where(attribute => attribute.Name == "href").FirstOrDefault().DeEntitizeValue;
                ret.Add(linkToArticle);
            }

            return ret;
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
