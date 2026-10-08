import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, forkJoin, Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { MyAuthService } from './auth-services/my-auth.service';

interface BackendWishListItem {
  id: number;
  productId: number;
  addedAt: string;
  productName: string;
  productDescription: string;
  productPicture: string;
  price: number;
  discountedPrice: number;
  isDiscounted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private wishList: any[] = [];
  private wishListCount = new BehaviorSubject<number>(0);
  wishListCount$ = this.wishListCount.asObservable();

  private getWishListUrl = 'https://localhost:7057/api/GetWishListEndpoint/my';
  private postWishListUrl = 'https://localhost:7057/api/PostWishListEndpoint';
  private deleteWishListUrl = 'https://localhost:7057/api/DeleteWishListEndpoint';

  constructor(
    private http: HttpClient,
    private authService: MyAuthService
  ) {}

  getWishList() {
    return this.wishList;
  }

  loadWishList(): Observable<any[]> {
    if (!this.shouldUseBackendWishList()) {
      this.resetState();
      return of([]);
    }

    return this.http.get<BackendWishListItem[]>(this.getWishListUrl).pipe(
      map(items => items.map(item => this.mapBackendItem(item))),
      tap(items => this.setWishList(items)),
      catchError(err => {
        console.error('Error loading wishlist from backend:', err);
        this.setWishList([]);
        return of([]);
      })
    );
  }

  addToWishList(product: any) {
    if (this.isProductInWishList(product.id)) {
      return;
    }

    if (!this.shouldUseBackendWishList()) {
      this.setWishList([...this.wishList, { ...product, quantity: 1 }]);
      return;
    }

    this.http.post(this.postWishListUrl, {
      productId: product.id
    }).subscribe({
      next: () => {
        this.setWishList([...this.wishList, { ...product, quantity: 1 }]);
      },
      error: (err) => {
        console.error('Error adding product to wishlist:', err);
        window.alert(err?.error || 'An error occurred while adding the product to wishlist.');
      }
    });
  }

  removeFromWishList(productId: number) {
    if (!this.shouldUseBackendWishList()) {
      this.setWishList(this.wishList.filter(p => p.id !== productId));
      return;
    }

    this.http.delete(`${this.deleteWishListUrl}/${productId}`).subscribe({
      next: () => {
        this.setWishList(this.wishList.filter(p => p.id !== productId));
      },
      error: (err) => {
        console.error('Error removing product from wishlist:', err);
        window.alert(err?.error || 'An error occurred while removing the product from wishlist.');
      }
    });
  }

  isProductInWishList(productId: number): boolean {
    return this.wishList.some(p => p.id === productId);
  }

  clearWishList() {
    if (!this.shouldUseBackendWishList()) {
      this.resetState();
      return;
    }

    const currentItems = [...this.wishList];
    this.setWishList([]);

    if (currentItems.length === 0) {
      return;
    }

    forkJoin(currentItems.map(item =>
      this.http.delete(`${this.deleteWishListUrl}/${item.id}`).pipe(
        catchError(() => of(null))
      )
    )).subscribe();
  }

  private shouldUseBackendWishList(): boolean {
    return this.authService.isLoggedIn() && this.authService.isCustomer();
  }

  resetState(): void {
    this.setWishList([]);
  }

  private setWishList(items: any[]): void {
    this.wishList = items;
    this.updateWishListCount();
  }

  private updateWishListCount() {
    this.wishListCount.next(this.wishList.length);
  }

  private mapBackendItem(item: BackendWishListItem): any {
    return {
      id: item.productId,
      name: item.productName,
      description: item.productDescription,
      picture: item.productPicture,
      price: Number(item.price),
      discountedPrice: item.isDiscounted ? Number(item.discountedPrice) : null,
      quantity: 1,
      addedAt: item.addedAt
    };
  }
}
