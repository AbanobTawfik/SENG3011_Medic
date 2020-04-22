// WHO
// https://www.codeonavirus.com/

import * as util from 'util';

import * as moment from 'moment';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class Codeonavirus extends ArticleApi
{
    constructor() {
        super('Codeonavirus',
              'WHO',
              'https://www.codeonavirus.com',
              '/who/articles');
    }

    ////////////////////////////////////////////////////////////////////
    // Building a Request

    public makeMethod()
    {
        return 'PUT';
    }

    public makeQueryString(startDate:  moment.Moment,
                           endDate:    moment.Moment,
                           keyTerms:   string[],
                           location:   string,
                           pageNumber: number): string
    {
        return '';
    }

    public makeBody(startDate:  moment.Moment,
                    endDate:    moment.Moment,
                    keyTerms:   string[],
                    location:   string,
                    pageNumber: number)
    {
        const body = {
            start_date: this.startDateValue(startDate),
            end_date: this.endDateValue(endDate),
        };
        if (keyTerms.length > 0) {
            body['key_terms'] = this.keyTermsValue(keyTerms);
        }
        if (location) {
            body['location'] = this.locationValue(location);
        }

        return {
            body: JSON.stringify(body),
        };
    }

    public makeHeaders()
    {
        return {
            'Content-Type': 'application/json',
        };
    }

    ////////////////////////////////////////////////////////////////////
    // Parameter Values

    public startDateValue(date: moment.Moment): string {
        return date.format('YYYY-MM-DD');
    }

    public endDateValue(date: moment.Moment): string {
        return date.format('YYYY-MM-DD');
    }

    public locationValue(location: string): string {
        return location + ','.repeat(3);
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

        return new StandardReport(diseases, syndromes, event_date,
                                  locations);
    }

    private toStandardLocation(resLocation)
    {
        const country = resLocation['country'];
        const state = resLocation['state'];
        const city = resLocation['city'];

        let location = '';
        if (city) {
            location += `, ${city}`;
        }

        // TODO: Find more robust way to get the city
        // if (state) {
        //     location += `, ${state}`;
        // }

        location = location.slice(2);
        return new StandardLocation(country, location);
    }

    ////////////////////////////////////////////////////////////////////
}

export default Codeonavirus;
