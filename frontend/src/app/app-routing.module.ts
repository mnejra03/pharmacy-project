import {NgModule} from '@angular/core';

import {RouterModule, Routes} from '@angular/router';
import {UnauthorizedComponent} from './modules/shared/unauthorized/unauthorized.component';
import {AuthGuard} from './auth-guards/auth-guard.service';

export const routes: Routes = [
  {path: 'unauthorized', component: UnauthorizedComponent},
  {
    path: 'admin',
    canActivate: [AuthGuard],
    data: {isAdmin: true}, 
    loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule)  // Lazy load  modula
  },
  {
    path: 'pharmacist',
    canActivate: [AuthGuard],
    data: {isPharmacist: true},
    loadChildren: () => import('./modules/pharmacist/pharmacist.module').then(m => m.PharmacistModule)  // Lazy load  modula
  },
  {
    path: 'public',
    loadChildren: () => import('./modules/public/public.module').then(m => m.PublicModule)  // Lazy load  modula
  },

  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule)  // Lazy load modula
  },

  {path: '', redirectTo: 'public', pathMatch: 'full'}  // Default ruta koja vodi na public
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule{
}
