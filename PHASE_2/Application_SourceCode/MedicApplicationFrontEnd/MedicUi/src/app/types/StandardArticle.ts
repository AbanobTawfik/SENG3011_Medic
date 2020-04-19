// Standard article object

import StandardReport from './StandardReport';

export class StandardArticle {
    url: string;
    dateOfPublication: string;
    headline: string;
    mainText: string;
    reports: StandardReport[];
    teamName: string;
    id;
    extra;

    constructor(url: string,
                dateOfPublication: string,
                headline: string,
                mainText: string,
                reports: StandardReport[],
                teamName: string = null,
                id = null,
                extra = null)
    {
        this.url = url;
        this.dateOfPublication = dateOfPublication;
        this.headline = headline;
        this.mainText = mainText;
        this.reports = reports;
        this.teamName = teamName;
        this.id = id;
        this.extra = extra;
    }
}

export default StandardArticle;
