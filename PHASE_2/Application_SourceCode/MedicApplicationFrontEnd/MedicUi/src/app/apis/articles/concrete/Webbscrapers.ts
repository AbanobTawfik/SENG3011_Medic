// WHO

import * as util from 'util';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

const AUTH_TOKEN = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImFiYzEyM0BnbWFpbC5jb20iLCJ0aW1lUmVnaXN0ZXJlZCI6IjAyLzA0LzIwMjAsIDE2OjQ1OjU2In0.-CH5qcrdDxqPSs94B-NrR3b8Q4B83maBYOUL9V0egSc'

class Webbscrapers extends ArticleApi
{
    constructor() {
        super("https://webbscrapers.live",
              "/reports");
    }

    ////////////////////////////////////////////////////////////////////
    // Building a Request

    public makeQueryString(startDate: Date,
                           endDate: Date,
                           keyTerms: string[],
                           location: string): string
    {
        let query =  `?startDateTime=${this.startDateValue(startDate)}` +
                     `&endDateTime=${this.endDateValue(endDate)}`;

        if (keyTerms && keyTerms.length > 0) {
            query += `&keyTerms=${this.keyTermsValue(keyTerms)}`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
        }

        return query;
    }

    public makeHeaders() {
        return {
            'Authorization': `Bearer ${AUTH_TOKEN}`,
        };
    }

    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    
    public processResponse(responseJson)
    {
        return this.getArticlesFromResponse(responseJson);
    }

    private getArticlesFromResponse(json)
    {
        if (typeof(json.result[0].Error) !== 'undefined') {
            return [];
        } else {
            // console.log(util.inspect(json, false, null, true));
            return json['result'].map(
                resArticle => this.toStandardArticle(resArticle)
            );
        }
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
        const geonamesId = resLocation['geonames-id'];
        return new StandardLocation(null, null, geonamesId);
    }

    ////////////////////////////////////////////////////////////////////
}

export default Webbscrapers;
