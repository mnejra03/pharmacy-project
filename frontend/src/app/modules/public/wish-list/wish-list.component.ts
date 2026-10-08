import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { WishlistService } from '../../../services/wishlist.service';
import { CartService } from '../../../services/cart.service';
import { CommonModule } from '@angular/common';
import { DecimalPipe } from '@angular/common';
import { concatMap } from 'rxjs/operators';
import { from } from 'rxjs';

@Component({
  selector: 'app-wish-list',
  templateUrl: './wish-list.component.html',
  imports: [
    CommonModule,
    DecimalPipe
  ],
  standalone: true,
  styleUrls: ['./wish-list.component.css']
})
export class WishListComponent implements OnInit {
  wishListProducts: any[] = [];
  toastMessage: string = '';
  showToastMessage: boolean = false;

  constructor(
    private wishlistService: WishlistService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWishlist();
  }

  loadWishlist(): void {
    this.wishlistService.loadWishList().subscribe({
      next: (items) => {
        this.wishListProducts = items;
      },
      error: (err) => {
        console.error('Error loading wishlist:', err);
      }
    });
  }

  navigateToProduct(id: number): void {
    this.router.navigate(['/public/product', id]);
  }

  clearWishlist(): void {
    this.wishlistService.clearWishList();
    this.wishListProducts = [];
  }
  showClearConfirmModal = false;

  showClearConfirm() {
    this.showClearConfirmModal = true;
  }

  confirmClearWishlist() {
    this.clearWishlist();
    this.showClearConfirmModal = false;
  }

  cancelClearWishlist() {
    this.showClearConfirmModal = false;
  }
  alreadyInCartMessage: string = '';
  showAlreadyInCartToast: boolean = false;

  moveToCart(): void {
    const productsToMove = this.wishListProducts.map(product => ({
      ...product,
      quantity: 1
    }));

    from(productsToMove).pipe(
      concatMap(product => this.cartService.addToCart(product))
    ).subscribe({
      next: () => {},
      error: (err) => {
        console.error('Error moving wishlist products to cart:', err);
        this.toastMessage = err?.error || 'Unable to move wishlist products to the cart.';
        this.showToastMessage = true;
      },
      complete: () => {
        this.closeConfirmationModal();
        this.showToastMessage = true;
        this.toastMessage = 'Wishlist products have been sent to the cart.';
        this.wishlistService.clearWishList();
        this.wishListProducts = [];

        setTimeout(() => {
          this.showToastMessage = false;
        }, 13000);
      }
    });
  }


  goToCart(): void {
    this.router.navigate(['/public/cart']);
  }

  openConfirmationModal() {
    this.showConfirmationModal = true;
  }
  showConfirmationModal = false;
  closeConfirmationModal() {
    this.showConfirmationModal = false;
  }
}
