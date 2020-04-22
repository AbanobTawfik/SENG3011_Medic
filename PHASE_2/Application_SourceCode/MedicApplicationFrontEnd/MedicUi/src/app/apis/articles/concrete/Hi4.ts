// Global Incident Map
// https://app.swaggerhub.com/apis/d-Nic/Hi-4_API/1.0.0-oas3

import * as util from 'util';

import * as moment from 'moment';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class Hi4 extends ArticleApi
{
    constructor() {
        super('Hi4',
              'Global Incident Map',
              'https://us-central1-seng3011-hi-4.cloudfunctions.net',
              '/app/reports');
    }

    ////////////////////////////////////////////////////////////////////
    // Building a Request

    public makeQueryString(startDate:  moment.Moment,
                           endDate:    moment.Moment,
                           keyTerms:   string[],
                           location:   string,
                           pageNumber: number): string
    {
        let query = `?start_date=${this.startDateValue(startDate)}` +
                    `&end_date=${this.endDateValue(endDate)}`;

        if (keyTerms && keyTerms.length > 0) {
            query += `&key_terms=${this.keyTermsValue(keyTerms)}`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
        }

        query += `&num=1000000`;
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
        const cases = resArticle['cases'];

        return new StandardArticle(url, dateOfPublication, headline,
                                   mainText, reports, this.name,
                                   this.source, null, { cases: cases });
    }

    private toStandardReport(resReport)
    {
        const diseases = resReport['diseases'];
        const syndromes = resReport['syndromes'];
        const event_date = resReport['event_date'];
        const locations = [
            this.toStandardLocation(resReport['locations'])
        ];

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

export default Hi4;

