import { Component } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import { Router } from '@angular/router';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cart: any[] = [];
  showBuyNowModal = false;

  constructor(
    private cartService: CartService,
    private router: Router,
    private authService: MyAuthService
  ) {}

  ngOnInit(): void {
    this.cartService.loadCart().subscribe({
      next: (items) => {
        this.cart = items;
      },
      error: (err) => {
        console.error('Error loading cart:', err);
      }
    });
  }

  navigateToProduct(id: number): void {
    this.router.navigate(['/public/product', id]);
  }

  updateItemTotal(item: any) {
    if (item.quantity < 1) item.quantity = 1;
    const unitPrice = item.discountedPrice && item.discountedPrice < item.price
      ? item.discountedPrice
      : item.price;
    item.totalPrice = unitPrice * item.quantity;
  }

  increaseQuantity(item: any): void {
    this.cartService.setQuantity(item.id, item.quantity + 1).subscribe({
      next: (updatedItem) => {
        if (updatedItem) {
          item.quantity = updatedItem.quantity;
          item.price = updatedItem.price;
          item.discountedPrice = updatedItem.discountedPrice;
          item.totalPrice = updatedItem.totalPrice;
        } else {
          item.quantity++;
          this.updateItemTotal(item);
        }
      },
      error: (err) => {
        console.error('Error increasing cart quantity:', err);
        alert(err?.error || 'Unable to update cart quantity.');
      }
    });
  }

  decreaseQuantity(item: any): void {
    if (item.quantity > 1) {
      this.cartService.setQuantity(item.id, item.quantity - 1).subscribe({
        next: (updatedItem) => {
          if (updatedItem) {
            item.quantity = updatedItem.quantity;
            item.price = updatedItem.price;
            item.discountedPrice = updatedItem.discountedPrice;
            item.totalPrice = updatedItem.totalPrice;
          } else {
            item.quantity--;
            this.updateItemTotal(item);
          }
        },
        error: (err) => {
          console.error('Error decreasing cart quantity:', err);
          alert(err?.error || 'Unable to update cart quantity.');
        }
      });
    }
  }

  removeFromCart(itemToRemove: any) {
    this.cartService.removeFromCart(itemToRemove.id).subscribe({
      next: () => {
        this.cart = this.cart.filter(item => item.id !== itemToRemove.id);
      },
      error: (err) => {
        console.error('Error removing item from cart:', err);
        alert(err?.error || 'Unable to remove item from cart.');
      }
    });
  }

  get totalPrice(): number {
    return this.cart.reduce((total, item) => {
      const unitPrice = item.discountedPrice && item.discountedPrice < item.price
        ? item.discountedPrice
        : item.price;
      return total + unitPrice * item.quantity;
    }, 0);
  }

  continueShopping(): void {
    this.router.navigate(['/public']);
  }


  buyNow() {

    if (!this.authService.isLoggedIn()) {
      localStorage.setItem('redirectAfterLogin', '/public/checkout');
      this.router.navigate(['/auth/login']);
      return;
    }
    if (!this.authService.isCustomer()) {
      alert('Only customers can place orders.');
      return;
    }
    this.router.navigate(['/public/checkout']);
  }

  protected readonly confirm = confirm;
}
