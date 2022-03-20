import { NgModule } from '@angular/core';
import { RouterModule, Routes} from "@angular/router";
import { HeroesComponent } from "./heroes/heroes.component";
import {DashboardComponent} from "./dashboard/dashboard.component";
import {HeroDetailComponent} from "./hero-detail/hero-detail.component";
import { UploadComponent } from "./upload/upload.component";
import {WeatherforecastComponent} from "./weatherforecast/weatherforecast.component";
import { NgForm } from "@angular/forms";

const routes: Routes = [
  { path: "", redirectTo: "/dashboard", pathMatch: "full" },
  { path: "hero", component:HeroesComponent },
  { path: "dashboard", component: DashboardComponent },
  /**
   * コロン (:) は:idが特定のヒーローのidのプレースホルダーであることを表しています
   * つまり、: はパラメータです
   */
  { path: "detail/:id", component: HeroDetailComponent },
  { path: "upload", component: UploadComponent},
  { path: "weatherforecast", component: WeatherforecastComponent}
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports:[RouterModule]
})
export class AppRoutingModule { }
