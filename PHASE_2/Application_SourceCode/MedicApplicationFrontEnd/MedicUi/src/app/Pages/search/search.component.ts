import { Component, OnInit } from '@angular/core';
import { StandardArticle } from '../../types/StandardArticle';
import { FormGroup, FormBuilder } from  '@angular/forms';
declare var $:any;
@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  searchForm: FormGroup;
  
  result: StandardArticle[] = [];
  a = new StandardArticle("https://www.cdc.gov/listeria/outbreaks/enoki-mushrooms-03-20/index.html", "2020-03-01 17:40:00", "Enoki Mushrooms - Listeria Infections", "36 people infected with the outbreak strain of Listeria monocytogenes have been reported from 17 states. Illnesses started on dates ranging from November 23, 2016 to December 13, 2019...", [], 1);
  
  constructor(private formBuilder: FormBuilder) {
    this.createSearchForm();

  }

  ngOnInit() {
    this.result.push(this.a);

  }

  createSearchForm(){
    this.searchForm = this.formBuilder.group({
      startDate: [''],
      endDate: [''],
      timezone: [''],
      location: [''],
      keyTerms: ['']
    });
  }

  onSubmit() {
    
    // console.log($('#datetimepicker1').datetimepicker());
    var mystartDate = window.document.getElementById("startDate").value;
    var myendDate = window.document.getElementById("endDate").value;
    console.log(mystartDate);
    console.log(myendDate);
    this.searchForm.value.startDate = mystartDate;
    this.searchForm.value.endDate = myendDate;
    console.log(this.searchForm.value); // Form Input is here
  }
}
