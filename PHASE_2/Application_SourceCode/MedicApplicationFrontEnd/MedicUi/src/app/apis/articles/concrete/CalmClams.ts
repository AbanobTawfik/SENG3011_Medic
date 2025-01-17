// Global Incident Map
// https://app.swaggerhub.com/apis-docs/SMEZ1234/SENG3011-CalmClams/1.0.0

import * as util from 'util';

import * as moment from 'moment';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class CalmClams extends ArticleApi
{
    constructor() {
        super('Calm Clams',
              'Global Incident Map',
              'http://calmclams.appspot.com',
              '/disease_reports');
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
        return json['articles'].map(
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
        const id = resArticle['_id'] || null;

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
        const country = resLocation['country'] || null;
        const location = resLocation['location'] || null;
        const geonames_id = resLocation['geonames_id'] || null;
        const google_id = resLocation['google_id'] || null;

        const coordinates = this.toCoordinates(resLocation['coords']);

        // TODO: Find more robust way to get the city
        let location2 = null;
        if (location) {
            location2 = location.replace(/,.*/, '');
        }

        return new StandardLocation(country, location2, geonames_id, 
                                    google_id, coordinates);
    }

    private toCoordinates(coordinatesString)
    {
        if (!coordinatesString) return null;
        if (coordinatesString.search(/, ?/) === -1) return null;

        const coordinates = coordinatesString.split(/, ?/);
        return {
            latitude: parseFloat(coordinates[0]),
            longitude: parseFloat(coordinates[1]),
        };
    }

    ////////////////////////////////////////////////////////////////////
}

export default CalmClams;
