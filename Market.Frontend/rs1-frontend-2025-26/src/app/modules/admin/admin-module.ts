import { NgModule } from '@angular/core';
import { AdminRoutingModule } from './admin-routing-module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';
import { SharedModule } from '../shared/shared-module';
import { UsersComponent } from './users/users.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdvertisementsComponent } from './advertisements/advertisements.component';
import { AdminProductsComponent } from './products/products.component';

@NgModule({
  declarations: [AdminLayoutComponent, AdminSettingsComponent, UsersComponent, DashboardComponent, AdvertisementsComponent, AdminProductsComponent],
  imports: [AdminRoutingModule, SharedModule]
})
export class AdminModule {}
