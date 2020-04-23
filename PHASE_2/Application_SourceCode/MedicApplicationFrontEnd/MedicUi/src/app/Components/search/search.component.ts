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
  keyterm: string = '';
  location = '';
  startDate: Date;
  endDate: Date;
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
  
  fromDate = null; // Date value from timepickers (use startDate and endDate instead)
  toDate = null;
  onDateChange() { // Update range when dates are changed and range is selected
    if (this.periodType == "range")
      this.changePeriod("range");
  }
  
  periodType: string = "week"
  displayPeriod: string = "Past week";
  changePeriod(type: string) {
    this.startDate = new Date();
    this.endDate = new Date();
    this.periodType = type;
    switch (type) {
      case "week":
        this.startDate.setDate(this.startDate.getDate() - 7); // Set to one week ago
        this.displayPeriod = "Past week";
        break;
      case "month":
        this.startDate.setMonth(this.startDate.getMonth() - 1); // Set to one month ago
        this.displayPeriod = "Past month";
        break;
      case "range":
        if (this.fromDate == null || this.toDate == null) { // Date validation
          this.startDate = this.endDate = null;
          this.displayPeriod = "Undefined range";
          return;
        }
        this.startDate.setFullYear(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
        this.endDate.setFullYear(this.toDate.year, this.toDate.month - 1, this.toDate.day);
        this.endDate.setHours(23, 59, 59, 999); // Set to end of day
        if (isNaN(this.startDate.getTime()) || isNaN(this.endDate.getTime()) || this.startDate > this.endDate) {
          this.startDate = this.endDate = null;
          this.displayPeriod = "Invalid range";
          return;
        }
        this.displayPeriod = this.startDate.toISOString().split('T')[0] + " to " + this.endDate.toISOString().split('T')[0];
    }
    this.startDate.setHours(0, 0, 0, 0); // Set to start of day
    console.log(moment.utc(this.startDate));
    console.log(moment.utc(this.endDate));
  }
}
