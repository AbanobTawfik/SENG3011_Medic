import { Component, OnInit } from '@angular/core';
import { ArticleRetrieverService } from '../../../Services/article-retriever.service';
import * as moment from "moment";
import articleStore from "../../apis/articles/interfaces/articleStore";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  keyterms = [];
  keyterm:string = '';
  location = '';
  startEnd = '';
  endDate = '';
  constructor(private articleService: ArticleRetrieverService) { }

  ngOnInit() {
  }

  addKeyTerm() {
    if (!(this.keyterm === '')) {
      this.keyterms.push(this.keyterm);
      this.keyterm = '';
    }
  }

  submitSearch(){
    var currentdate = moment();
    var previousweek = currentdate.subtract(3, "w");
    const articleRequests = articleStore.createRequests(
      moment.utc([2020, 0, 1, 0, 0, 0]),
      moment.utc([2020, 1, 1, 0, 0, 0]),
      this.keyterms,
      this.location,
      []
    );
    this.articleService.modifyStatus(true);
    this.articleService.CreateRequest(articleRequests);
  }

  expandInput(event) {
    event.target.parentNode.getElementsByTagName("span")[0].innerText = event.target.value;
  }

  onFromSelect(event) {
    document.getElementById("from").dispatchEvent(new Event("input"));
  }

  onToSelect(event) {
    document.getElementById("to").dispatchEvent(new Event("input"));
  }
  
  openFocus(event, input) {
    if (event)
      setTimeout(() => input.focus(), 10);
  }
}
