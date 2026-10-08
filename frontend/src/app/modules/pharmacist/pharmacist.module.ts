import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PharmacistRoutingModule } from './pharmacist-routing.module';
import { ParmacistLayoutComponent } from './parmacist-layout/parmacist-layout.component';
import { PharmacistDashboardComponent } from './pharmacist-dashboard/pharmacist-dashboard.component';
import {FormsModule} from '@angular/forms';
import { StockComponent } from './stock/stock.component';
import { RecipesComponent } from './recipes/recipes.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { MyOrdersComponent } from './my-orders/my-orders.component';
import { CustomerOrderComponent } from './customer-order/customer-order.component';


@NgModule({
  declarations: [
    ParmacistLayoutComponent,
    PharmacistDashboardComponent,
    StockComponent,
    RecipesComponent,
    NotificationsComponent,
    MyOrdersComponent,
    CustomerOrderComponent
  ],
  imports: [
    CommonModule,
    PharmacistRoutingModule,
    FormsModule
  ]
})
export class PharmacistModule { }
