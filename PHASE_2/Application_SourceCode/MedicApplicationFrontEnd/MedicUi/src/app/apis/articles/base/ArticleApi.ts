
import * as util from 'util';

import * as moment from 'moment';
import * as fetch from 'node-fetch';

import * as colors from 'colors';

import StandardArticle from '../../../types/StandardArticle';

abstract class ArticleApi {
    name:     string;
    url:      string;
    endpoint: string;

    limit:    number = null; // number of articles to fetch at a time

    constructor(url: string, endpoint: string) {
        this.url = url;
        this.endpoint = endpoint;
    }

    ////////////////////////////////////////////////////////////////////
    // Getters

    public supportsPaging()
    {
        return false;
    }

    ////////////////////////////////////////////////////////////////////
    // Fetching Articles

    /**
     * 
     * @param startDate  - the start date in UTC time
     * @param endDate    - the end date in UTC time
     * @param keyTerms   - an array of key terms
     * @param location   - a location
     * @param pageNumber - zero-indexed page number
     */
    async fetchArticles(startDate:  moment.Moment,
                        endDate:    moment.Moment,
                        keyTerms:   string[] = [],
                        location:   string   = "",
                        pageNumber: number   = null)
    {
        if (!location) location = "";

        let method = this.makeMethod();

        let query = this.makeQueryString(startDate, endDate, keyTerms,
                                         location, pageNumber);

        if (query.length > 0) {
            console.log(
                colors.cyan('[INFO]') +
                colors.green('[' + `API: ${this.name}`.padEnd(22) + `] `) +
                `Query string:  ${query}`
            );
        }
        
        let body = this.makeBody(startDate, endDate, keyTerms, location,
                                 pageNumber);

        if (Object.keys(body).length > 0) {
            console.log(
                colors.cyan('[INFO]') +
                colors.green('[' + `API: ${this.name}`.padEnd(22) + `] `) +
                `Body:          ${body['body']}`
            );
        }

        let headers = this.makeHeaders();

        if (Object.keys(body).length > 0) {
            console.log(
                colors.cyan('[INFO]') +
                colors.green('[' + `API: ${this.name}`.padEnd(22) + `] `) +
                'Headers:       ' +
                util.inspect(headers, false, null, false)
            );
        }

        let request = `${this.url}${this.endpoint}${query}`;

        let response;
        try {
            response = await fetch(
                request,
                {
                    method: method,
                    ...body,
                    headers: {
                        'Accept': 'application/json',
                        ...headers,
                    },
                },
            );
        } catch (e) {
            console.log(
                colors.red('[API ERROR]') +
                colors.green('[' + `API: ${this.name}`.padEnd(22) + `] `) +
                e
            );
            return [];
        }
        
        let responseJson = await response.json();

        try {
            return this.processResponse(responseJson);
        } catch (e) {
            console.log(
                colors.red('[PROCESSING ERROR]') +
                colors.green('[' + `API: ${this.name}`.padEnd(22) + `] `) +
                e
            );
            return [];
        }
    }

    ////////////////////////////////////////////////////////////////////
    // Building a Request

    public makeMethod(): string {
        return 'GET';
    }

    public abstract makeQueryString(startDate:  moment.Moment,
                                    endDate:    moment.Moment,
                                    keyTerms:   string[],
                                    location:   string,
                                    pageNumber: number): string;
    
    public makeBody(startDate:  moment.Moment,
                    endDate:    moment.Moment,
                    keyTerms:   string[],
                    location:   string,
                    pageNumber: number)
    {
        return {};
    }

    public makeHeaders(): object {
        return {};
    }
    
    ////////////////////////////////////////////////////////////////////
    // Parameter Values

    public startDateValue(date: moment.Moment): string {
        return date.format('YYYY-MM-DDTHH:mm:ss');
    }

    public endDateValue(date: moment.Moment): string {
        return date.format('YYYY-MM-DDTHH:mm:ss');
    }

    public locationValue(location: string): string {
        return location;
    }

    public keyTermsValue(keyTerms: string[]): string {
        return keyTerms.join(',');
    }

    ////////////////////////////////////////////////////////////////////
    // Post-Processing

    public abstract processResponse(responseJson);

    public getNextParameters(startDate: moment.Moment,
                             endDate:   moment.Moment,
                             articles:  StandardArticle[])
    {
        return null;
    }

    ////////////////////////////////////////////////////////////////////
}

export default ArticleApi;
