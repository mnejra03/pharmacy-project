import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {DiscountedProductsService} from '../../../services/discounted-products.service';
import { HttpClient } from '@angular/common/http';
import {Router} from '@angular/router';
import {WishlistService} from '../../../services/wishlist.service';
import {CartService} from '../../../services/cart.service';

interface DiscountedProduct {
  id:number;
  name: string;
  description: string;
  price: number;
  quantityInStock: number;
  picture: string;
  isDiscounted: boolean;
  discountPercentage: number;
  discountedPrice?: number;
}


@Component({
  selector: 'app-discounted-products',
  templateUrl: './discounted-products.component.html',
  standalone: false,
  styleUrls: ['./discounted-products.component.css']
})
export class DiscountedProductsComponent implements OnInit {
  //discountedProducts: any[] = []; // Lista sniženih proizvoda
  discountedProducts: DiscountedProduct[] = [];

  constructor(private productService: DiscountedProductsService,
              private router: Router,
              private wishListService: WishlistService,
              private cartService: CartService,
              private cdr: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.loadDiscountedProducts();
  }

  loadDiscountedProducts() {
    this.productService.getDiscountedProducts().subscribe((data: any[]) => {
      this.discountedProducts = data.map(product => {
        const discountedPrice = product.isDiscounted && product.discountPercentage
          ? product.price - (product.price * product.discountPercentage / 100)
          : product.price;

        return {
          ...product,
          discountedPrice
        } as DiscountedProduct;
      });
    });
  }


  currentIndex = 0;
  visibleCards = 4;
  cardWidth = 250;
  transformValue = 'translateX(0px)';

  toastMessage: string = '';
  showToastMessage: boolean = false;
  nextProduct() {
    const maxIndex = this.discountedProducts.length - this.visibleCards;
    if (this.currentIndex < maxIndex) {
      this.currentIndex++;
      this.updateTransform();
    }
  }

  previousProduct() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.updateTransform();
    }
  }

  updateTransform() {
    const shift = -this.currentIndex * this.cardWidth;
    this.transformValue = `translateX(${shift}px)`;
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/public/product', productId]);
  }


  isInWishList(product: any): boolean {
    return this.wishListService.isProductInWishList(product.id);
  }

  navigateToCart() {
    this.router.navigate(['/public/cart']);
  }
  navigateToWishList() {
    this.router.navigate(['/public/wish-list']);
  }


  toastRedirect: 'cart' | 'wishlist' | null = null;

  showToast(message: string, redirect: 'cart' | 'wishlist' | null = null) {
    this.toastMessage = message;
    this.toastRedirect = redirect;
    this.showToastMessage = true;
    this.cdr.detectChanges();

    setTimeout(() => {
      this.showToastMessage = false;
      this.toastRedirect = null;
      this.cdr.detectChanges();
    }, 3000);
  }

  toggleWishList(product: any, event: MouseEvent) {
    event.stopPropagation();

    if (this.isInWishList(product)) {
      this.wishListService.removeFromWishList(product.id);
    } else {
      this.wishListService.addToWishList(product);
      this.showToast('Product added to wishlist!', 'wishlist');
    }
  }

  addToCart(product: any): void {
    product.quantity = 1;
    this.cartService.addToCart(product).subscribe({
      next: () => {
        this.showToast(`${product.name} has been added to the cart.`, 'cart');
      },
      error: (err) => {
        console.error('Error adding product to cart:', err);
        this.showToast(err?.error || 'An error occurred while adding the product to cart.', null);
      }
    });
  }

}






