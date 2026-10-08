import { NgModule } from '@angular/core'; import { RouterModule, Routes } from '@angular/router';
import { NotificationsComponent } from './notifications/notifications.component'; import { RecipesComponent } from './recipes/recipes.component'; import { PharmacistRecipesComponent } from './recipes/pharmacist-recipes.component'; import { ChatComponent } from './chat/chat.component'; import { ProfileComponent } from './profile/profile.component';
import { ClientLayoutComponent } from './client-layout/client-layout.component';
const routes: Routes = [{ path:'', component:ClientLayoutComponent, children:[
  { path: 'notifications', component: NotificationsComponent }, { path: 'recipes', component: RecipesComponent },
  { path: 'pharmacist/recipes', component: PharmacistRecipesComponent },
  { path: 'chat', component: ChatComponent }, { path: 'profile', component: ProfileComponent },
  { path: '', redirectTo: 'profile', pathMatch: 'full' }
]}];
@NgModule({ imports:[RouterModule.forChild(routes)], exports:[RouterModule] }) export class ClientRoutingModule {}
