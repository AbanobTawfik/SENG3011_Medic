// CDC

import * as util from 'util';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class FlyingSplaucers extends ArticleApi
{
    constructor() {
        super("https://us-central1-flyingsplaucers-7b3cf.cloudfunctions.net",
              "/reports/reports");
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
            query += `&key_terms=${this.keyTermsValue(keyTerms)}`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
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
        // console.log(util.inspect(json, false, null, true));
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
        const reports = [this.toStandardReport(resArticle['reports'])];

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

export default FlyingSplaucers;
