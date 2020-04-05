// Standard article object

import StandardReport from './StandardReport';

export class StandardArticle {
    url: string;
    dateOfPublication: string;
    headline: string;
    mainText: string;
    reports: StandardReport[];
    id;

    constructor(url: string,
                dateOfPublication: string,
                headline: string,
                mainText: string,
                reports: StandardReport[],
                id = null)
    {
        this.url = url;
        this.dateOfPublication = dateOfPublication;
        this.headline = headline;
        this.mainText = mainText;
        this.reports = reports;
        this.id = id;
    }
}

export default StandardArticle;
