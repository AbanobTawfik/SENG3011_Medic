// ProMed
// https://seng3011.mikuray.cf/api/v1/

import * as util from 'util';

import * as moment from 'moment';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class Fm extends ArticleApi
{
    lastIndex: number;

    constructor() {
        super('Fm',
              'ProMed',
              'https://seng3011.mikuray.cf',
              '/api/v1/articles/reports');

        this.limit = 200;
        this.lastIndex = 0;
    }

    ////////////////////////////////////////////////////////////////////
    // Getters

    public supportsPaging()
    {
        return true;
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

        query += `&limit=${this.limit}`;
        query += `&after=${this.lastIndex}`;
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
        json.forEach(resArticle => {
            // console.log(resArticle.index);
            if (resArticle.index > this.lastIndex) {
                this.lastIndex = resArticle.index;
            }
        });
        
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
        const id = resArticle['id'];

        return new StandardArticle(url, dateOfPublication, headline,
                                   mainText, reports, this.name,
                                   this.source, id);
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
        const country = resLocation['country'];
        const location = resLocation['location'];
        const googleId = resLocation['google_id'];

        // TODO: Find more robust way to get the city
        const location2 = location.replace(/,.*/, '');
        
        return new StandardLocation(country, location2, null, googleId);
    }

    ////////////////////////////////////////////////////////////////////
}

export default Fm;
