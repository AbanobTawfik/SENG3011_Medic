using MedicApi.Models;
using NLog;
using System;

namespace MedicApi.Services
{
    public class APILogger
    {
        public Logger logger; 
        public APILogger()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }

        public void LogReceive(string start_date, string end_date, string timezone, string key_terms, string location, string max, string offset)
        {
            var requestBody = new
            {
                start_date = start_date,
                end_date = end_date,
                timezone = timezone,
                key_terms = key_terms,
                location = location,
                max = max,
                offset = offset
            };
            var requestBodyString = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var logString = "\n\n======================================\nRequest received " +
                            requestBodyString + "\n";
            this.logger.Info(logString);
        }

        public void LogErrors(ApiGetArticlesError errors, string timeTakenForRequest)
        {
            var logString = "\n\nThe following fields were not included; BAD REQUEST\n";
            foreach(var error in errors.errors.Keys)
            {
                logString += "MISSING FIELD <" + error + ">" + ": " + errors.errors[error] + "\n"; 
            }
            logString += "\nTotal Time Taken For Request: " + timeTakenForRequest + "\n======================================\n\n";
            this.logger.Error(logString);
        }

        public void LogSuccess(Article[] article, string timeTakenForRequest)
        {
            var logString = "\nThe Query has returned the following results; SUCCESS\n";
            logString += Newtonsoft.Json.JsonConvert.SerializeObject(article) + "\n";
            logString += "\nTotal Time Taken For Request: " + timeTakenForRequest + "\n======================================\n\n";
            this.logger.Info(logString);
        }
    }
}
