import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";
import { environment } from "../environments/environment";
import * as util from "util";
import articleStore from "../app/apis/articles/interfaces/articleStore";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class ArticleRetrieverService {
  // get all articles from all apis stored into here
  private isSearch = new BehaviorSubject(false);
  private RequestData = new BehaviorSubject(null);
  currentStatus = this.isSearch.asObservable();
  currentRequest = this.RequestData.asObservable();

  modifyStatus(status: boolean){
    this.isSearch.next(status);
  }


  CreateRequest(request){
    this.RequestData.next(request);
  }

}