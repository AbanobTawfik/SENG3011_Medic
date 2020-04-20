import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ChartsModule } from 'ng2-charts';

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { NavBarComponent } from "./Components/nav/nav-bar.component";
import { HomeComponent } from "./Pages/home/home.component";
import { SearchComponent } from "./Pages/search/search.component";
import { SummaryComponent } from "./Pages/summary/summary.component";
import { MapComponent } from "./Components/map/map.component";
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

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    SearchComponent,
    SummaryComponent,
    MapComponent,
    MapArticlesPopupComponent,
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
    FlexLayoutModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
