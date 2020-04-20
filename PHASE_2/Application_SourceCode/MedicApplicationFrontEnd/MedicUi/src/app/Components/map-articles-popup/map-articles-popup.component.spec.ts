import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MapArticlesPopupComponent } from './map-articles-popup.component';

describe('MapArticlesPopupComponent', () => {
  let component: MapArticlesPopupComponent;
  let fixture: ComponentFixture<MapArticlesPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MapArticlesPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MapArticlesPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
