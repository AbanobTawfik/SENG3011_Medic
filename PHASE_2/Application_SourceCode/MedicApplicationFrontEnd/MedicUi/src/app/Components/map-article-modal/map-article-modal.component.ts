import { Component, Input, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import StandardArticle from '../../types/StandardArticle';

@Component({
  selector: 'app-map-article-modal',
  templateUrl: './map-article-modal.component.html',
  styleUrls: ['./map-article-modal.component.scss']
})
export class MapArticleModalComponent implements OnInit {

  @Input() public article: StandardArticle;

  constructor(private http: HttpClient, private modalService: NgbModal) { }

  ngOnInit() {
    console.log(this.article.headline);
    this.loadArticleCases(this.article);
  }

  loadArticleCases(article) {
    if (article.source != "CDC") return;
    console.log(article);
    this.http.get(
      "https://localhost:5003/api/Reports/GetCases?url=" + article.url, {
        headers: new HttpHeaders({
          "Content-Type": "application/json"
        }),
        observe: 'response'
      }
    )
    .subscribe(response => {
      if (response.status != 200)
        return;
      else
        article.model = true;
    }, err => {
      console.log(err);
    });
  }
}
