import { Component, Input, OnInit } from '@angular/core';

import StandardArticle from "../../types/StandardArticle";

@Component({
  selector: 'app-map-articles-popup',
  templateUrl: './map-articles-popup.component.html',
  styleUrls: ['./map-articles-popup.component.scss']
})
export class MapArticlesPopupComponent implements OnInit {

  @Input() articles: StandardArticle[];

  greetings: string[];

  constructor() { }

  ngOnInit() {
    this.greetings = [
      'hello',
      'bonjour',
      'hola',
      'konnichiwa',
      'ni hao',
    ];
  }

}
