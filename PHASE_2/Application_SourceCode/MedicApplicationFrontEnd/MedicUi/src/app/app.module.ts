import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { NavBarComponent } from "./Components/nav/nav-bar.component";
import { HomeComponent } from "./Pages/home/home.component";
import { SearchComponent } from "./Pages/search/search.component";
import { SummaryComponent } from "./Pages/summary/summary.component";
import { MapComponent } from "./Components/map/map.component";
import { AgmCoreModule } from "@agm/core";
import { DlDateTimeDateModule, DlDateTimePickerModule } from 'angular-bootstrap-datetimepicker';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { environment } from "../environments/environment";
import { HttpClientModule } from "@angular/common/http";
import { ChartModule, HIGHCHARTS_MODULES } from 'angular-highcharts';

import exporting from 'highcharts/modules/exporting.src';
import windbarb from 'highcharts/modules/windbarb.src';

export function highchartsModules() {
  // apply Highcharts Modules to this array
  return [ exporting,windbarb ];
}

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    SearchComponent,
    SummaryComponent,
    MapComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    DlDateTimeDateModule,
    DlDateTimePickerModule,
    ChartModule,
    AgmCoreModule.forRoot({
      apiKey: environment.GoogleApiKey,
    }),
  ],
  providers: [
    { provide: HIGHCHARTS_MODULES, useFactory: highchartsModules } // add as factory to your providers
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
