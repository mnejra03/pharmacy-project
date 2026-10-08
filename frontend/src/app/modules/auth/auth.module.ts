import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AuthRoutingModule} from './auth-routing.module';
import {LoginComponent} from './login/login.component';
import {FormsModule} from '@angular/forms';
import {MatButton} from "@angular/material/button";
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {TranslatePipe} from "@ngx-translate/core";
import {SharedModule} from "../shared/shared.module";
import { AuthLayoutComponent } from './auth-layout/auth-layout.component';
import { RegisterComponent } from './register/register.component';


@NgModule({
  declarations: [
    LoginComponent,
    AuthLayoutComponent,

  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    FormsModule,
    MatButton,
    MatSlideToggle,
    TranslatePipe,
    SharedModule,
    RegisterComponent
  ]
})
export class AuthModule{
}

