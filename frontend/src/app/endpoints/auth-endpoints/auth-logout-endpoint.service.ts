import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { MyConfig } from '../../my-config';
import { MyAuthService } from '../../services/auth-services/my-auth.service';
import { ChatService } from '../../services/chat.service';
import { Router } from '@angular/router';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';

@Injectable({
  providedIn: 'root'
})
export class AuthLogoutEndpointService {
  private apiUrl = `${MyConfig.api_address}/auth/logout`;

  constructor(
    private httpClient: HttpClient,
    private authService: MyAuthService,
    private chatService: ChatService,
    private router: Router,
    private wishlistService: WishlistService,
    private cartService: CartService
  ) {}

  handleAsync(): Observable<void> {
    return this.httpClient.post<void>(this.apiUrl, {}).pipe(
      catchError(() => of(void 0)),
      finalize(() => {
        this.chatService.stopConnection();
        this.wishlistService.resetState();
        this.cartService.resetState();
        this.authService.setLoggedInUser(null);
        this.router.navigate(['/auth/login']);
      })
    );
  }
}
