import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router} from '@angular/router';
import {MyAuthService} from '../services/auth-services/my-auth.service';

export class AuthGuardData {
  isAdmin?: boolean;
  isPharmacist?: boolean;
  isCustomer?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: MyAuthService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const guardData = route.data as AuthGuardData;
    const requiresRole = !!(guardData.isAdmin || guardData.isPharmacist || guardData.isCustomer);

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    if (!requiresRole) {
      return true;
    }

    const hasAllowedRole =
      (guardData.isAdmin && this.authService.isAdmin()) ||
      (guardData.isPharmacist && this.authService.isPharmacist()) ||
      (guardData.isCustomer && this.authService.isCustomer());

    if (!hasAllowedRole) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }

}
