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
  private listView = new BehaviorSubject(null);
  private isUpdatedMap = new BehaviorSubject(false);
  currentMapSearched = this.isUpdatedMap.asObservable();
  currentStatus = this.isSearch.asObservable();
  currentRequest = this.RequestData.asObservable();
  currentListView = this.listView.asObservable();

  modifyStatus(status: boolean){
    this.isSearch.next(status);
  }


  CreateRequest(request){
    this.RequestData.next(request);
  }

  updateListView(list){
    this.listView.next(list);
  }

  updateMapStatus(status: boolean){
    this.isUpdatedMap.next(status);
  }
}