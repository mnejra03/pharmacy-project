import { NgModule } from '@angular/core';
import { PublicRoutingModule } from './public-routing-module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SharedModule } from '../shared/shared-module';
import { CatalogComponent } from './catalog/catalog.component';
import { ProductDetailsComponent } from './product-details/product-details.component';

@NgModule({ declarations: [PublicLayoutComponent, CatalogComponent, ProductDetailsComponent], imports: [SharedModule, PublicRoutingModule] })
export class PublicModule {}
