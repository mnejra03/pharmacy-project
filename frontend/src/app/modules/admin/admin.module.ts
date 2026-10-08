import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './products/products.component';
import { UsersComponent } from './users/users.component';
import { OrdersComponent } from './orders/orders.component';
import { MediaComponent } from './media/media.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {BaseChartDirective} from 'ng2-charts';
import {Chart} from 'chart.js';
import {LineController, CategoryScale, LinearScale, Title, Tooltip, Legend, PointElement, LineElement,Filler } from 'chart.js';

Chart.register(LineController, CategoryScale, LinearScale, Title, Tooltip, Legend, PointElement, LineElement,Filler);

@NgModule({
  declarations: [
    DashboardComponent,
    AdminLayoutComponent,
    ProductsComponent,
    UsersComponent,
    OrdersComponent,
    MediaComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    BaseChartDirective,
    ReactiveFormsModule

  ]
})
export class AdminModule {

}
