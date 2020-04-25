import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./Pages/home/home.component";
import { SearchComponent } from "./Components/search/search.component";
import { SummaryComponent } from "./Pages/summary/summary.component";
import { ModelComponent } from "./Pages/model/model.component";
import { NavBarComponent } from "./Components/nav/nav-bar.component";

const routes: Routes = [
  { path: "home", component: HomeComponent },
  { path: "search", component: SearchComponent },
  { path: "summary", component: SummaryComponent },
  { path: "model", component: ModelComponent },
  { path: "", redirectTo: "home", pathMatch: "full" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
