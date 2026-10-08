import { Injectable } from '@angular/core';
import { BehaviorSubject, forkJoin, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { MyAuthService } from './auth-services/my-auth.service';

interface BackendCartItem {
  productId: number;
  productName: string;
  productDescription: string;
  productPicture: string;
  quantity: number;
  price: number;
  total: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly guestCartStorageKey = 'guest-cart-items';
  private cartItems: any[] = [];
  private cartItemCount = new BehaviorSubject<number>(0);
  readonly cartItemCount$ = this.cartItemCount.asObservable();

  private addToCartUrl = 'https://localhost:7057/api/PostAddToCartEndpoint';
  private deleteFromCartUrl = 'https://localhost:7057/api/DeleteFromCartEndpoint';
  private getActiveCartUrl = 'https://localhost:7057/api/GetActiveCartEndpoint/active';

  constructor(
    private http: HttpClient,
    private authService: MyAuthService
  ) {
    this.setCartItems(this.readGuestCartItems());
  }

  getCart(): any[] {
    return this.cartItems;
  }

  loadCart(): Observable<any[]> {
    if (!this.shouldUseBackendCart()) {
      const guestItems = this.readGuestCartItems();
      this.setCartItems(guestItems);
      return of(guestItems);
    }

    return this.syncGuestCartToBackendIfNeeded().pipe(
      switchMap(() => this.http.get<BackendCartItem[]>(this.getActiveCartUrl)),
      map(items => items.map(item => this.mapBackendItem(item))),
      tap(items => {
        this.clearGuestCartStorage();
        this.setCartItems(items);
      }),
      catchError(err => {
        console.error('Error loading cart from backend:', err);
        this.setCartItems([]);
        return of([]);
      })
    );
  }

  addToCart(item: any): Observable<void> {
    const quantity = item.quantity || 1;

    if (!this.shouldUseBackendCart()) {
      const nextItems = [...this.cartItems];
      const existingItem = nextItems.find(cartItem => cartItem.id === item.id);

      if (existingItem) {
        existingItem.quantity += quantity;
        existingItem.totalPrice = this.calculateItemTotal(existingItem);
      } else {
        nextItems.push({
          ...item,
          quantity,
          totalPrice: this.calculateItemTotal({ ...item, quantity })
        });
      }

      this.setCartItems(nextItems);
      return of(void 0);
    }

    return this.http.post(this.addToCartUrl, {
      productId: item.id,
      quantity
    }).pipe(
      switchMap(() => this.loadCart()),
      map(() => void 0)
    );
  }

  removeFromCart(productId: number): Observable<void> {
    if (!this.shouldUseBackendCart()) {
      this.setCartItems(this.cartItems.filter(item => item.id !== productId));
      return of(void 0);
    }

    return this.http.delete<void>(`${this.deleteFromCartUrl}/${productId}`).pipe(
      tap(() => {
        this.setCartItems(this.cartItems.filter(item => item.id !== productId));
      })
    );
  }

  setQuantity(productId: number, quantity: number): Observable<any> {
    if (quantity < 1) {
      return this.removeFromCart(productId);
    }

    if (!this.shouldUseBackendCart()) {
      const nextItems = [...this.cartItems];
      const item = nextItems.find(x => x.id === productId);
      if (item) {
        item.quantity = quantity;
        item.totalPrice = this.calculateItemTotal(item);
        this.setCartItems(nextItems);
      }
      return of(item);
    }

    const existingItem = this.cartItems.find(item => item.id === productId);
    const removeExistingItem$ = existingItem
      ? this.http.delete<void>(`${this.deleteFromCartUrl}/${productId}`)
      : of(void 0);

    return removeExistingItem$.pipe(
      switchMap(() => this.http.post(this.addToCartUrl, {
        productId,
        quantity
      })),
      switchMap(() => this.loadCart()),
      map(items => items.find(item => item.id === productId))
    );
  }

  clearCart(): void {
    if (!this.shouldUseBackendCart()) {
      this.setCartItems([]);
      return;
    }

    const currentItems = [...this.cartItems];
    this.setCartItems([]);

    if (currentItems.length === 0) {
      return;
    }

    forkJoin(currentItems.map(item =>
      this.http.delete(`${this.deleteFromCartUrl}/${item.id}`).pipe(
        catchError(() => of(null))
      )
    )).subscribe();
  }

  private shouldUseBackendCart(): boolean {
    return this.authService.isLoggedIn() && this.authService.isCustomer();
  }

  resetState(): void {
    this.setCartItems([]);
  }

  private setCartItems(items: any[]): void {
    this.cartItems = items;
    if (this.shouldUseBackendCart()) {
      this.clearGuestCartStorage();
    } else {
      this.persistGuestCartItems(items);
    }
    this.updateCartCount();
  }

  private updateCartCount(): void {
    const totalItems = this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
    this.cartItemCount.next(totalItems);
  }

  private mapBackendItem(item: BackendCartItem): any {
    return {
      id: item.productId,
      name: item.productName,
      description: item.productDescription,
      picture: item.productPicture,
      price: Number(item.price),
      discountedPrice: null,
      quantity: item.quantity,
      totalPrice: Number(item.total)
    };
  }

  private calculateItemTotal(item: any): number {
    const unitPrice = item.discountedPrice && item.discountedPrice < item.price
      ? item.discountedPrice
      : item.price;

    return unitPrice * item.quantity;
  }

  private persistGuestCartItems(items: any[]): void {
    localStorage.setItem(this.guestCartStorageKey, JSON.stringify(items));
  }

  private readGuestCartItems(): any[] {
    const storedCart = localStorage.getItem(this.guestCartStorageKey);

    if (!storedCart) {
      return [];
    }

    try {
      const parsedCart = JSON.parse(storedCart);
      return Array.isArray(parsedCart) ? parsedCart : [];
    } catch {
      return [];
    }
  }

  private clearGuestCartStorage(): void {
    localStorage.removeItem(this.guestCartStorageKey);
  }

  private syncGuestCartToBackendIfNeeded(): Observable<void> {
    const guestItems = this.readGuestCartItems();

    if (guestItems.length === 0) {
      return of(void 0);
    }

    return forkJoin(
      guestItems.map(item => this.http.post(this.addToCartUrl, {
        productId: item.id,
        quantity: item.quantity
      }))
    ).pipe(
      tap(() => this.clearGuestCartStorage()),
      map(() => void 0),
      catchError(err => {
        console.error('Error syncing guest cart to backend:', err);
        return of(void 0);
      })
    );
  }
}
