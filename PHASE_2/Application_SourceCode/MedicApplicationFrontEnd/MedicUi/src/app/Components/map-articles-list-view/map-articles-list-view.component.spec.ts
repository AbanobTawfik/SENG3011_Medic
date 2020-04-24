import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MapArticlesListViewComponent } from './map-articles-list-view.component';

describe('MapArticlesListViewComponent', () => {
  let component: MapArticlesListViewComponent;
  let fixture: ComponentFixture<MapArticlesListViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MapArticlesListViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MapArticlesListViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
