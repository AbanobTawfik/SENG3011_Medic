// Flu Tracker

import * as util from 'util';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class ExceptionalCoders extends ArticleApi
{
    constructor() {
        super("https://us-central1-seng3011-859af.cloudfunctions.net",
              "/app/api/v1/articles");
    }

    ////////////////////////////////////////////////////////////////////
    // Building a Request

    public makeQueryString(startDate: Date,
                           endDate: Date,
                           keyTerms: string[],
                           location: string): string
    {
        let query =  `?start_date=${this.startDateValue(startDate)}` +
                     `&end_date=${this.endDateValue(endDate)}`;

        if (keyTerms && keyTerms.length > 0) {
            query += `&keyterms=${this.keyTermsValue(keyTerms)}`;
        } else {
            query += `&keyterms=`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
        } else {
            query += `&location=`;
        }

        return query;
    }

    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    
    public processResponse(responseJson)
    {
        return this.getArticlesFromResponse(responseJson);
    }

    private getArticlesFromResponse(json)
    {
        console.log(util.inspect(json, false, null, true));
        return json.map(
            resArticle => this.toStandardArticle(resArticle)
        );
    }

    private toStandardArticle(resArticle)
    {
        const url = resArticle['url'];
        const dateOfPublication = resArticle['date_of_publication'];
        const headline = resArticle['headline'];
        const mainText = resArticle['main_text'];
        const reports = resArticle['reports'].map(
            resReport => this.toStandardReport(resReport)
        );

        return new StandardArticle(url, dateOfPublication, headline,
                                   mainText, reports);
    }

    private toStandardReport(resReport)
    {
        const diseases = resReport['diseases'];
        const syndromes = resReport['syndromes'];
        const event_date = resReport['event_date'];
        const locations = resReport['locations'].map(
            resLocation => this.toStandardLocation(resLocation)
        );

        return new StandardReport(diseases, syndromes, event_date,
                                  locations);
    }

    private toStandardLocation(resLocation)
    {
        const country = resLocation['country'];
        const location = resLocation['location'];
        return new StandardLocation(country, location);
    }

    ////////////////////////////////////////////////////////////////////
}

export default ExceptionalCoders;
