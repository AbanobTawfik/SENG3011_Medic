import { Component, Input, OnInit } from '@angular/core';

import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';

import StandardArticle from "../../types/StandardArticle";

import { MapArticleModalComponent } from '../map-article-modal/map-article-modal.component';
import StandardReport from '../../types/StandardReport';
import StandardLocation from '../../types/StandardLocation';

@Component({
  selector: 'app-map-articles-popup',
  templateUrl: './map-articles-popup.component.html',
  styleUrls: ['./map-articles-popup.component.scss']
})
export class MapArticlesPopupComponent implements OnInit {

  articlesReal: StandardArticle[] = [];
  @Input() articles: StandardArticle[];

  loaded = false;
  constructor(config: NgbModalConfig, private modalService: NgbModal) {
    config.centered = true;
    config.size = "lg";
  }

  ngOnInit() {
    if (this.articlesReal = [] && this.articles) {
      this.articlesReal = [];
      this.articles.forEach(x => {
        var add = new StandardArticle(x.url, x.dateOfPublicationStr, x.headline, x.mainText, x.reports, x.teamName, x.source, x.id, x.extra);
        this.articlesReal.push(add);
      })
    }
  }

  checkDateOfPub(article: StandardArticle){
    return article.formatDateOfPublication() !== "";
  }

  testClick(str: string = '???') {
    console.log(`You clicked on '${str}'`);
  }

  openArticleModal(article: StandardArticle) {
    console.log(this.articlesReal);
    const modalRef = this.modalService.open(MapArticleModalComponent);
    modalRef.componentInstance.article = article;
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
