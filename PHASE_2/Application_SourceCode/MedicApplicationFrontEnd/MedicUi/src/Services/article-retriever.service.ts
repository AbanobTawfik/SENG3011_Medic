import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";
import { environment } from "../environments/environment";
import * as util from "util";
import articleStore from "../app/apis/articles/interfaces/articleStore";

@Injectable({
  providedIn: "root",
})
export class ArticleRetrieverService {
  // get all articles from all apis stored into here
  articles: any[];

  
}

// articles: any[] = [];
// constructor(private http: HttpClient) {}

// getAllArticles() {
//   this.getOwnArticles();
//   return this.articles;
// }

// async getOwnArticles() {
//   const searchParams = new HttpParams({
//     fromString:
//       "start_date=1999-01-01T00%3A00%3A00&end_date=2022-01-01T00%3A00%3A00",
//   });
//   const options = { params: searchParams };
//   await this.http
//     .get<any>(environment.MedicEndPoint, options)
//     .subscribe((res) => {
//       for (let article of res["articles"]) {
//         this.articles.push(article);
//       }
//     });
// }
