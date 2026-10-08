import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {BeautyAndCareComponent} from './beauty-and-care/beauty-and-care.component';
import {ChildcareComponent} from './childcare/childcare.component';
import {DevicesComponent} from './devices/devices.component';
import {NewInComponent} from './new-in/new-in.component';
import {SkinProtectionComponent} from './skin-protection/skin-protection.component';
import {YourHealthComponent} from './your-health/your-health.component';
import {PublicLayoutComponent} from './public-layout/public-layout.component';
import {HomeComponent} from './home/home.component';
import {LoginComponent} from '../auth/login/login.component';
import {WishListComponent} from './wish-list/wish-list.component';
import {SearchResultsComponent} from './search-results/search-results.component';
import {ProductDetailsComponent} from './product-details/product-details.component';
import {ShippingInfoComponent} from './shipping-info/shipping-info.component';
import {BrandsComponent} from './brands/brands.component';
import {BrandsProductComponent} from './brands-products/brands-products.component';
import {CartComponent} from './cart/cart.component';
import {CheckoutComponent} from './checkout/checkout.component';
import {RecipesComponent} from '../pharmacist/recipes/recipes.component';
import {RecipeComponent} from './recipe/recipe.component';
import {MyOrdersComponent} from './my-orders/my-orders.component';
import {AuthGuard} from '../../auth-guards/auth-guard.service';

const routes: Routes = [
  {
    path: '', component: PublicLayoutComponent, children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'beauty-and-care', component: BeautyAndCareComponent },
      { path: 'childcare', component: ChildcareComponent },
      { path: 'checkout', component: CheckoutComponent },
      { path: 'devices', component: DevicesComponent },
      { path: 'new-in', component: NewInComponent },
      { path: 'skin-protection', component: SkinProtectionComponent },
      { path: 'your-health', component: YourHealthComponent },
      { path: 'shipping-info', component: ShippingInfoComponent },
      { path: 'search-results', component: SearchResultsComponent },
      { path: 'login', component: LoginComponent },
      { path: 'wish-list', component: WishListComponent },
      { path: 'cart', component: CartComponent },
      { path: 'recipe', component: RecipeComponent },
      { path: 'product/:id', component: ProductDetailsComponent },
      { path: 'brands-products/:id', component: BrandsProductComponent },
      {path: 'my-orders', component: MyOrdersComponent, canActivate: [AuthGuard], data: { isCustomer: true }},
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ]
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule{}
