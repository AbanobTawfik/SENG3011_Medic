// CIDRAP
// http://13.55.197.137/

import * as util from 'util';

import * as moment from 'moment';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class GnarlyNarwhals extends ArticleApi
{
    constructor() {
        super('Gnarly Narwhals',
              'CIDRAP',
              'http://13.55.197.137',
              '/data');
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
        const reports = resArticle['reports'].map(
            resReport => this.toStandardReport(resReport)
        );

        return new StandardArticle(url, dateOfPublication, headline,
                                   mainText, reports, this.name,
                                   this.source);
    }

    private toStandardReport(resReport)
    {
        const diseases = resReport['diseases'];
        const syndromes = resReport['syndromes'];
        const event_date = resReport['event_date'];
        const locations = resReport['locations'].map(
            resLocation => this.toStandardLocation(resLocation)
        );

        const normalisedDiseases = diseases.map(this.normaliseDisease);

        return new StandardReport(normalisedDiseases, syndromes, event_date,
                                  locations);
    }

    private toStandardLocation(resLocation)
    {
        const geonamesId = parseInt(resLocation['geonames_id']);
        
        return new StandardLocation(null, null, geonamesId);
    }

    ////////////////////////////////////////////////////////////////////
}

export default GnarlyNarwhals;
