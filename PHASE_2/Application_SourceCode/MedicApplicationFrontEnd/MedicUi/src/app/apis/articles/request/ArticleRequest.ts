
import ArticleApi from '../base/ArticleApi';

class ArticleRequest
{
    startDate: Date;
    endDate:   Date;
    keyTerms:  string[];
    location:  string;
    api:       ArticleApi;

    constructor(startDate: Date,
                endDate:   Date,
                keyTerms:  string[],
                location:  string,
                api:       ArticleApi)
    {
        this.startDate = startDate;
        this.endDate   = endDate;
        this.keyTerms  = keyTerms;
        this.location  = location;
        this.api       = api;
    }

    async fetchFirst() {
        return this.api.fetchArticles(this.startDate, this.endDate,
                                      this.keyTerms, this.location);
    }

    async fetchMore() {
        return [];
    }
}

export default ArticleRequest;
