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
import { environment } from "../environments/environment";
import { HttpClientModule } from "@angular/common/http";

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
    AgmCoreModule.forRoot({
      apiKey: environment.GoogleApiKey,
    }),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
