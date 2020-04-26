import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ChartsModule } from 'ng2-charts';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { NavBarComponent } from "./Components/nav/nav-bar.component";
import { HomeComponent } from "./Pages/home/home.component";
import { SummaryComponent } from "./Pages/summary/summary.component";
import { ModelComponent } from "./Pages/model/model.component";
import { MapComponent } from "./Components/map/map.component";
import { SearchComponent } from "./Components/search/search.component";
import { AgmCoreModule } from "@agm/core";
import {
  DlDateTimeDateModule,
  DlDateTimePickerModule,
} from "angular-bootstrap-datetimepicker";
import { environment } from "../environments/environment";
import { HttpClientModule } from "@angular/common/http";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { FlexLayoutModule } from '@angular/flex-layout';
import { MapArticlesPopupComponent } from './Components/map-articles-popup/map-articles-popup.component';
import { MapArticleModalComponent } from './Components/map-article-modal/map-article-modal.component';
import { MapArticlesListViewComponent } from './Components/map-articles-list-view/map-articles-list-view.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    SummaryComponent,
    ModelComponent,
    MapComponent,
    MapArticlesPopupComponent,
    MapArticleModalComponent,
    SearchComponent,
    MapArticlesListViewComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    DlDateTimeDateModule,
    DlDateTimePickerModule,
    AgmCoreModule.forRoot({
      apiKey: environment.GoogleApiKey,
    }),
    NgMultiSelectDropDownModule.forRoot(),
    FlexLayoutModule,
    FontAwesomeModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [
    MapArticleModalComponent
  ],
})
export class AppModule { }
