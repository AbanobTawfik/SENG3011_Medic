import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  keyterms = [];
  keyterm = '';
  locations = [];
  location = '';
  startEnd = '';
  endDate = '';
  constructor() { }

  ngOnInit() {
  }

  addKeyTerm() {
    if (!(this.keyterm === '')) {
      this.keyterms.push(this.keyterm);
      this.keyterm = '';
    }
  }

  addLocation(){
    if (!(this.location === '')) {
      this.locations.push(this.location);
      this.location = '';
    }
    console.log(this.locations);
  }

  submitSearch(){
    console.log("sent");
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
}
