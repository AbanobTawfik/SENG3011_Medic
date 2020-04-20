// Standard article object

import * as moment from 'moment';

import StandardReport from './StandardReport';
import dateUtils from '../utils/dateParser';

export class StandardArticle {
    url: string;
    dateOfPublicationStr: string;
    dateOfPublication: moment.Moment;
    headline: string;
    mainText: string;
    reports: StandardReport[];
    teamName: string;
    id;
    extra;

    constructor(url: string,
                dateOfPublicationStr: string,
                headline: string,
                mainText: string,
                reports: StandardReport[],
                teamName: string = null,
                id = null,
                extra = null)
    {
        this.url = url;
        this.dateOfPublicationStr = dateOfPublicationStr;
        this.dateOfPublication = dateUtils.xStrToDateFloor(dateOfPublicationStr);
        this.headline = headline;
        this.mainText = mainText;
        this.reports = reports;
        this.teamName = teamName;
        this.id = id;
        this.extra = extra;
    }

    formatDateOfPublication() {
        if (this.dateOfPublication === null) {
            console.log("BAD DATE OF PUBLICATION: " + this.dateOfPublicationStr +
                        " " + this.teamName + " " + this.url);
            return "";
        } else {
            return this.dateOfPublication.format("DD MMM YYYY");
        }
    }
}

export default StandardArticle;
