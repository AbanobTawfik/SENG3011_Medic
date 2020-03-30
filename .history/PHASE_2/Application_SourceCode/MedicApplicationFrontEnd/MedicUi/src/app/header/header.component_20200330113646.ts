import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  categories: any[] = ["Home", "Search", "Prediction"];
  isCollapsed = false;

  constructor() { }

  ngOnInit() {
  }

  toggleMenu(){
    this.isCollapsed = !this.isCollapsed;
  }

}
