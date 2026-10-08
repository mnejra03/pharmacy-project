import {ChangeDetectorRef, Component} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ProductServicesService} from '../../../services/product-services.service';
import {NewInProductsService} from '../../../services/new-in-products.service';
import {Router} from '@angular/router';
import {WishlistService} from '../../../services/wishlist.service';
import {CartService} from '../../../services/cart.service';

interface NewProduct {
  name: string;
  description: string;
  price: number;
  quantityInStock: number;
  picture: string;
  isDiscounted: boolean;
  discountPercentage: number;
  discountedPrice?: number;
}

interface NewInProduct {
  id: number;
  name: string;
  picture: string;
  price: number;
  dateAdded: string;
}

@Component({
  selector: 'app-new-in',
  standalone: false,
  templateUrl: './new-in.component.html',
  styleUrl: './new-in.component.css'
})
export class NewInComponent {
  latestProducts: NewInProduct[] = [];

  currentIndex = 0;
  visibleCards = 4;
  cardWidth = 250;
  transformValue = 'translateX(0px)';

  constructor(private productService: NewInProductsService, private router: Router,
              private wishListService: WishlistService,
              private cartService: CartService,
              private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadLatestProducts();
  }

  loadLatestProducts() {
    this.productService.getLatestProducts().subscribe(data => {
      this.latestProducts = data;
    });
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/public/product', productId]);
  }

  toastMessage: string = '';
  showToastMessage: boolean = false;

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
