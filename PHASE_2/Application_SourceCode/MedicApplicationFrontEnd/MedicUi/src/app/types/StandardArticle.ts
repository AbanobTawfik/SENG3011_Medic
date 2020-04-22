// Standard article object

import * as moment from 'moment';

import StandardReport from './StandardReport';
import dateUtils from '../utils/dateParser';
import textUtils from '../utils/textUtils';

export class StandardArticle {
    url: string;
    dateOfPublicationStr: string;
    dateOfPublication: moment.Moment;
    headline: string;
    mainText: string;
    reports: StandardReport[];
    teamName: string;
    source: string;
    id;
    extra;

    constructor(url: string,
                dateOfPublicationStr: string,
                headline: string,
                mainText: string,
                reports: StandardReport[],
                teamName: string = null,
                source: string = null,
                id = null,
                extra = null)
    {
        this.url = url;
        this.dateOfPublicationStr = dateOfPublicationStr;
        this.dateOfPublication = dateUtils.xStrToDateFloor(dateOfPublicationStr);
        this.headline = headline;
        this.mainText = textUtils.removeExcessSpace(mainText);
        this.reports = reports;
        this.teamName = teamName;
        this.source = source;
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

    generatePreview() {
        return textUtils.getFirstWords(this.mainText, 75);
    }
}

export default StandardArticle;
