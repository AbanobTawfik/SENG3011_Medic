import { Component, OnInit, Input } from '@angular/core';

import StandardArticle from "../../types/StandardArticle";
import { ArticleRetrieverService } from '../../../Services/article-retriever.service';

@Component({
  selector: 'app-map-articles-list-view',
  templateUrl: './map-articles-list-view.component.html',
  styleUrls: ['./map-articles-list-view.component.scss']
})
export class MapArticlesListViewComponent implements OnInit {

  articlesReal: StandardArticle[] = [];
  @Input() articles: StandardArticle[];

  constructor(private articleRetriever: ArticleRetrieverService) { }

  ngOnChanges() {
    if (!this.compareArticles()) {
      this.articlesReal = [];
      this.articles.forEach(x => {
        var add = new StandardArticle(x.url, x.dateOfPublicationStr, x.headline, x.mainText, x.reports, x.teamName, x.source, x.id, x.extra);
        this.articlesReal.push(add);
      });
    }
  }

  compareArticles() {
    return JSON.stringify(this.articles) == JSON.stringify(this.articlesReal);
  }

  ngOnInit() {
    if (this.articlesReal.length === 0 && this.articles) {
      this.articlesReal = [];
      this.articles.forEach(x => {
        var add = new StandardArticle(x.url, x.dateOfPublicationStr, x.headline, x.mainText, x.reports, x.teamName, x.source, x.id, x.extra);
        this.articlesReal.push(add);
      });
    }

    // this.articleRetriever.currentListView.subscribe(articles => {
    //   if (articles != null) {
    //     articles.array.forEach(article => {
    //       console.log(article);
    //     });
    //   }
    // })

  }

  getPopupArticles() {
    const sorted = this.sortedArticles();
    return sorted;
  }

  sortedArticles() {
    if (this.articlesReal) {
      const sorted = this.articlesReal.slice();
      sorted.sort((a, b) => {
        const aPubDate = a.dateOfPublication;
        const bPubDate = b.dateOfPublication;

        if (aPubDate === null) {
          return -1;
        } else if (bPubDate === null) {
          return 1;
        } else if (aPubDate < bPubDate) {
          return -1;
        } else if (aPubDate == bPubDate) {
          return 0;
        } else {
          return 1;
        }
      });
      sorted.reverse();
      return sorted;
    }
  }
}
