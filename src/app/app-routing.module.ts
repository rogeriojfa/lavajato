import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./components/views/home/home.component";
import { ClienteReadComponent } from './components/views/cliente/cliente-read/cliente-read.component';

const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
  },
  {
    path: "clientes",
    component: ClienteReadComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {} 
