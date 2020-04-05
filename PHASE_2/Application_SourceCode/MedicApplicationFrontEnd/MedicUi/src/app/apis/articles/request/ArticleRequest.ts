// Wrapper around ArticleApi that handles pagination

import * as moment from 'moment';

import ArticleApi from '../base/ArticleApi';

import StandardArticle from '../../../types/StandardArticle';

class ArticleRequest
{
    startDate:  moment.Moment;
    endDate:    moment.Moment;
    keyTerms:   string[];
    location:   string;
    pageNumber: number = 0;
    
    api:        ArticleApi;

    seen:       Set<string> = new Set();
    done:       boolean = false;

    constructor(startDate: moment.Moment,
                endDate:   moment.Moment,
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

    ////////////////////////////////////////////////////////////////////
    // Getters

    public getApiName(): string {
        return this.api.name;
    }

    ////////////////////////////////////////////////////////////////////
    // Fetching Articles

    /**
     * Fetch all articles satisfying the request.  Subsequent calls will
     * return an empty list.
     */
    public async fetchAll(): Promise<StandardArticle[]> {
        const articles = [];
        let someArticles = await this.fetchMore();
        while (someArticles.length > 0) {
            articles.push(...someArticles);
            someArticles = await this.fetchMore();
        }
        return articles;
    }

    /**
     * Fetch more articles satisfying the request.  Repeatedly call this
     * to retrieve all articles that satisfy the request. Will return an
     * empty list if there are no more articles satisfying the request.
     */
    public async fetchMore(): Promise<StandardArticle[]> {
        if (this.done) return [];

        // fetch articles from API
        const articles = await this.api.fetchArticles(
            this.startDate, this.endDate, this.keyTerms,
            this.location, this.pageNumber
        );

        // if no more articles
        if (articles.length == 0) {
            this.seen = new Set();
            this.done = true;
            return [];
        }

        // avoid returning duplicates
        const result = articles.filter(article =>
            !this.seen.has(article.url + article.dateOfPublication)
        )
        for (let article of result) {
            this.seen.add(article.url + article.dateOfPublication);
        }

        this.setNextParameters(result);
        return result;
    }

    ////////////////////////////////////////////////////////////////////

    // Set next parameters for the next call to fetch
    private setNextParameters(articles: StandardArticle[])
    {
        if (this.api.supportsPaging()) {
            this.pageNumber += 1;
        } else {
            const next = this.api.getNextParameters(
                this.startDate, this.endDate, articles
            );

            if (next === null) {
                this.done = true;
                return;
            }

            this.startDate = next.startDate;
            this.endDate = next.endDate;
        }
    }

    ////////////////////////////////////////////////////////////////////
}

export default ArticleRequest;
