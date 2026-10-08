import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {AdminLayoutComponent} from '../admin/admin-layout/admin-layout.component';
import {AuthGuard} from '../../auth-guards/auth-guard.service';
import {DashboardComponent} from '../admin/dashboard/dashboard.component';
import {UsersComponent} from '../admin/users/users.component';
import {ProductsComponent} from '../admin/products/products.component';
import {OrdersComponent} from '../admin/orders/orders.component';
import {PublicLayoutComponent} from '../public/public-layout/public-layout.component';
import {PharmacistDashboardComponent} from './pharmacist-dashboard/pharmacist-dashboard.component';
import {ParmacistLayoutComponent} from  './parmacist-layout/parmacist-layout.component';
import {StockComponent} from './stock/stock.component';
import {RecipesComponent} from './recipes/recipes.component';
import {NotificationsComponent} from './notifications/notifications.component';
import {MyOrdersComponent} from './my-orders/my-orders.component';
import {CustomerOrderComponent} from './customer-order/customer-order.component';


const routes: Routes = [
  {
    path: '',
    component: ParmacistLayoutComponent,
    canActivate: [AuthGuard],
    data: { isPharmacist: true },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: PharmacistDashboardComponent },
      { path: 'stock', component: StockComponent },
      { path: 'recipes', component: RecipesComponent },
      { path: 'notifications', component: NotificationsComponent },
      {path:'my-orders', component: MyOrdersComponent },
      {path:'customer-order', component: CustomerOrderComponent },
      { path: '**', redirectTo: 'dashboard' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],

  exports: [RouterModule]

})
export class PharmacistRoutingModule { }
