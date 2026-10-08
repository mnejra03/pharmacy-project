import {ChangeDetectorRef, Component} from '@angular/core';
import {ProductServicesService, Product} from '../../../services/product-services.service';
import {Router} from '@angular/router';
import {WishlistService} from '../../../services/wishlist.service';
import {CartService} from '../../../services/cart.service';


@Component({
  selector: 'app-skin-protection',
  standalone: false,
  templateUrl: './skin-protection.component.html',
  styleUrl: './skin-protection.component.css'
})
export class SkinProtectionComponent {
  products: Product[] = [];

  constructor(private productServices: ProductServicesService,
              private router: Router,
              private wishListService: WishlistService,
              private cartService: CartService,
              private cdr: ChangeDetectorRef) {}
  goToProduct(productId: number) {
    this.router.navigate(['/public/product', productId]);
  }
  ngOnInit(): void {
    // Specifična logika za učitavanje proizvoda iz kategorije "Your Health"
    this.productServices.getProductsByCategory(4).subscribe(products => { // ID "1" za "Your Health"
      this.products = products;
    });
  }
  selectedSortOption: string = ''; // ✅ inicijalizacija bez greške

  get sortedProducts(): Product[] {
    const productsCopy = [...this.products];

    switch (this.selectedSortOption) {
      case 'priceAsc':
        return productsCopy.sort((a, b) => a.price - b.price);
      case 'priceDesc':
        return productsCopy.sort((a, b) => b.price - a.price);
      case 'nameAsc':
        return productsCopy.sort((a, b) => a.name.localeCompare(b.name));
      case 'nameDesc':
        return productsCopy.sort((a, b) => b.name.localeCompare(a.name));
      default:
        return productsCopy;
    }
  }
  getDiscountedPrice(product: Product): number {
    if (product.isDiscounted && product.discountPercentage) {
      return product.price - (product.price * product.discountPercentage / 100);
    }
    return product.price;
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
