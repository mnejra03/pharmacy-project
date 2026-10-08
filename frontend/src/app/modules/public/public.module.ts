import {LOCALE_ID, NgModule } from '@angular/core';
import {CommonModule, registerLocaleData} from '@angular/common';
import { PublicRoutingModule } from './public-routing.module';
import { BeautyAndCareComponent } from './beauty-and-care/beauty-and-care.component';
import { ChildcareComponent } from './childcare/childcare.component';
import { DevicesComponent } from './devices/devices.component';
import { NewInComponent } from './new-in/new-in.component';
import { SkinProtectionComponent } from './skin-protection/skin-protection.component';
import { YourHealthComponent } from './your-health/your-health.component';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { HomeComponent } from './home/home.component';
import {MatIconModule} from '@angular/material/icon';
import { WishListComponent } from './wish-list/wish-list.component';
import { AdvertisementComponent } from './advertisement/advertisement.component';
import { DiscountedProductsComponent } from './discounted-products/discounted-products.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { SearchResultsComponent } from './search-results/search-results.component';
import {ProductDetailsComponent} from './product-details/product-details.component';
import { ShipBannerComponent } from './ship-banner/ship-banner.component';
import { ShippingInfoComponent } from './shipping-info/shipping-info.component';
import { BrandsComponent } from './brands/brands.component';
import { BrandsProductComponent } from './brands-products/brands-products.component';
import { CartComponent } from './cart/cart.component';
import { CheckoutComponent } from './checkout/checkout.component';
import localeBa from '@angular/common/locales/bs';
import { ChatComponent } from './chat/chat.component';
import { RecipeComponent } from './recipe/recipe.component';
import { MyOrdersComponent } from './my-orders/my-orders.component'; // import bosnian locale

registerLocaleData(localeBa, 'bs'); // Register the bosnian locale

@NgModule({
  declarations: [
    BeautyAndCareComponent,
    ChildcareComponent,
    DevicesComponent,
    NewInComponent,
    SkinProtectionComponent,
    YourHealthComponent,
    PublicLayoutComponent,
    HomeComponent,
    AdvertisementComponent,
    DiscountedProductsComponent,
    SearchResultsComponent,
    ProductDetailsComponent,
    ShipBannerComponent,
    ShippingInfoComponent,
    BrandsComponent,
    BrandsProductComponent,
    CartComponent,
    CheckoutComponent,
    ChatComponent,
    RecipeComponent,
    MyOrdersComponent
  ],
    imports: [
        CommonModule,
        PublicRoutingModule,
        MatIconModule,
        FormsModule,
        WishListComponent,
        ReactiveFormsModule,


    ],
  providers: [
    { provide: LOCALE_ID, useValue: 'bs' }
  ],
})
export class PublicModule { }
