// H5N1

import StandardArticle from '../../../types/StandardArticle';
import StandardLocation from '../../../types/StandardLocation';
import StandardReport from '../../../types/StandardReport';
import ArticleApi from '../base/ArticleApi';

class APIdemic extends ArticleApi
{
    constructor() {
        super("http://api-demic.herokuapp.com",
              "/v1.1/articles");
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
            query += `&key_term=${this.keyTermsValue(keyTerms)}`;
        }

        if (location) {
            query += `&location=${this.locationValue(location)}`;
        }

        query += `&limit=10`;

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

        return new StandardArticle(url, dateOfPublication, headline,
                                   mainText, reports);
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

        return new StandardReport(diseases, syndromes, event_date,
                                  locations);
    }

    private toStandardLocation(resLocation)
    {
        return new StandardLocation(resLocation['country'],
                                    resLocation['location']);
    }

    ////////////////////////////////////////////////////////////////////
}

export default APIdemic;
