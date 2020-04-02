import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  categories: any[] = [
    {name: 'Home'},
    {name: 'Popular Games'},
    {name: 'Table Games'},
    {name: 'Other Games'}
  ];
  isCollapsed = false;

  constructor() { }

  ngOnInit() {
  }

  toggleMenu(){
    this.isCollapsed = !this.isCollapsed;
  }

}
