import { Component, Input, OnInit } from '@angular/core';

import StandardArticle from '../../types/StandardArticle';

@Component({
  selector: 'app-map-article-modal',
  templateUrl: './map-article-modal.component.html',
  styleUrls: ['./map-article-modal.component.scss']
})
export class MapArticleModalComponent implements OnInit {

  @Input() public article: StandardArticle;

  constructor() { }

  ngOnInit() {
    console.log(this.article.headline);
  }

}
