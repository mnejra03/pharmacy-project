import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MyAuthInfo } from './dto/my-auth-info';
import { LoginTokenDto } from './dto/login-token-dto';

@Injectable({ providedIn: 'root' })
export class MyAuthService {
  constructor(
    private httpClient: HttpClient,
    private router: Router
  ) {}

  getMyAuthInfo(): MyAuthInfo | null {
    return this.getLoginToken()?.myAuthInfo ?? null;
  }

  isLoggedIn(): boolean {
    return this.getMyAuthInfo() != null && this.getMyAuthInfo()!.isLoggedIn;
  }

  isAdmin(): boolean {
    return this.getMyAuthInfo()?.isAdmin ?? false;
  }

  isPharmacist(): boolean {
    return this.getMyAuthInfo()?.isPharmacist ?? false;
  }

  isCustomer(): boolean {
    return this.getMyAuthInfo()?.isCustomer ?? false;
  }

  setLoggedInUser(x: LoginTokenDto | null) {
    if (x == null) {
      window.localStorage.setItem('my-auth-token', '');
    } else {
      window.localStorage.setItem('my-auth-token', JSON.stringify(x));
    }
  }

  getCurrentUserId(): number | null {
    return this.getMyAuthInfo()?.userId ?? null;
  }

  getCurrentUserEmail(): string | null {
    return this.getMyAuthInfo()?.email ?? null;
  }

  getCurrentUserFullName(): string {
    const authInfo = this.getMyAuthInfo();
    if (!authInfo) return '';
    return `${authInfo.firstName || ''} ${authInfo.lastName || ''}`.trim();
  }

  getLoginToken(): LoginTokenDto | null {
    const tokenString = window.localStorage.getItem('my-auth-token') ?? '';
    try {
      return JSON.parse(tokenString);
    } catch {
      return null;
    }
  }

  loginSuccessful(): void {
    const redirectUrl = localStorage.getItem('redirectAfterLogin');
    if (redirectUrl) {
      localStorage.removeItem('redirectAfterLogin');
      this.router.navigate([redirectUrl]);
    } else {
      this.router.navigate(['/']);
    }
  }

  logout(): void {
    this.setLoggedInUser(null);
    localStorage.removeItem('redirectAfterLogin');
    this.router.navigate(['/auth/login']);
  }
}
