// H5N1
// http://api-demic.herokuapp.com

import * as util from 'util';

import * as moment from 'moment';

import dateUtils from '../utils/dateUtils';

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class APIdemic extends ArticleApi
{
    constructor() {
        super('ApiDemic',
              'H5N1',
              'http://api-demic.herokuapp.com',
              '/v1.1/articles');

        this.limit = 250;
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
            query += `&key_term=${this.keyTermsValue(keyTerms)}`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
        }

        query += `&limit=${this.limit}`;

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
        if (json.status === 404) {
            return [];
        } else {
            return json['articles'].map(
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
                                   mainText, reports, this.name,
                                   this.source);
    }

    private toStandardReport(resReport)
    {
        const diseases = [];
        if (typeof(resReport['disease']) === "string") {
            if (resReport['disease'] !== '') {
                diseases.push(resReport['disease']);
            }
        } else if (typeof(resReport['diseases']) === "object") {
            diseases.push(...resReport['diseases']);
        }

        const syndromes = [];
        if (typeof(resReport['syndrome']) === "string") {
            if (resReport['syndrome'] !== '') {
                syndromes.push(resReport['syndrome']);
            }
        } else if (typeof(resReport['syndromes']) === "object") {
            syndromes.push(...resReport['syndromes']);
        }

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

        // TODO: Find more robust way to get the city
        const location2 = location.replace(/,.*/, '');

        return new StandardLocation(country, location2);
    }

    ////////////////////////////////////////////////////////////////////

    public getNextParameters(startDate: moment.Moment,
                             endDate:   moment.Moment,
                             articles:  StandardArticle[])
    {
        const a = articles[articles.length - 1];
        const lastDate = dateUtils.xStrToDateFloor(a.dateOfPublicationStr);
        const newStartDate = lastDate.add(1, 's');
        return {
            startDate: newStartDate,
            endDate:   endDate,
        };
    }

    ////////////////////////////////////////////////////////////////////
}

export default APIdemic;
