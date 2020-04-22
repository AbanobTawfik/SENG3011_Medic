import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MapArticleModalComponent } from './map-article-modal.component';

describe('MapArticleModalComponent', () => {
  let component: MapArticleModalComponent;
  let fixture: ComponentFixture<MapArticleModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MapArticleModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MapArticleModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
