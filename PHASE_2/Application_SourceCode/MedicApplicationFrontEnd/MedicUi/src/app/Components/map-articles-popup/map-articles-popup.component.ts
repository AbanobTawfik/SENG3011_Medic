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

  @Input() articles: StandardArticle[];
  loaded = false;
  constructor(config: NgbModalConfig, private modalService: NgbModal) {
    config.centered = true;
    config.size = "lg";
  }

  ngOnInit() {
    
  }

  testClick(str: string = '???') {
    console.log(`You clicked on '${str}'`);
  }

  openArticleModal(article: StandardArticle) {
    alert(JSON.stringify(article));
    const modalRef = this.modalService.open(MapArticleModalComponent);
    modalRef.componentInstance.article = article;
  }

  getPopupArticles() {
    // console.log(this.articles.length);
    // const sorted = this.sortedArticles();
    //console.log(JSON.stringify(sorted));
    return this.articles;
  }

  sortedArticles() {
    const sorted = this.articles.slice();
    sorted.sort((a, b) => {
      const aPubDate = a.dateOfPublication;
      const bPubDate = b.dateOfPublication;

      if (aPubDate === null) {
        return -1;
      } else if (bPubDate === null) {
        return  1;
      } else if (aPubDate < bPubDate) {
        return -1;
      } else if (aPubDate == bPubDate) {
        return  0;
      } else {
        return  1;
      }
    });
    sorted.reverse();
    return sorted;
  }
}
