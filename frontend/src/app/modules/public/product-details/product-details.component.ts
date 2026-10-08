import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductServicesService, Product } from '../../../services/product-services.service';
import { CartService } from '../../../services/cart.service';
import { ReviewService } from '../../../services/review.service';

export interface Review {
  id?: number;
  userName: string;
  text: string;
  rating: number;
  productId: number;
}

@Component({
  selector: 'app-product-details',
  standalone: false,
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit {
  productId = 0;
  product!: Product;
  reviews: Review[] = [];
  newReview: Review = { userName: '', text: '', rating: 5, productId: 0 };

  showReviewForm = false;
  showError = false;
  showSimilarProducts = false;
  showReviews = false;

  similarProducts: Product[] = [];
  quantity = 1;
  toastMessage = '';
  showToastMessage = false;
  toastRedirect: 'cart' | 'wishlist' | 'error' | 'success' | null = null;
  showImageZoom = false;

  @ViewChild('reviewForm') reviewFormElement!: ElementRef;
  @ViewChild('topElement') topElement: ElementRef | undefined;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductServicesService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private cartService: CartService,
    private reviewService: ReviewService,
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.productId = +params['id'];
      this.loadProduct(this.productId);
      this.getReviews();
    });
  }

  loadProduct(id: number) {
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product = product;
        this.newReview.productId = product.id;
        this.loadSimilarProducts();
      },
      error: () => {
        this.showError = true;
      }
    });
  }

  loadSimilarProducts() {
    if (!this.product?.name) return;

    const keyword = this.product.name.split(' ')[0];
    this.productService.getSimilarProducts(keyword, this.product.id).subscribe({
      next: (products) => {
        this.similarProducts = products.filter(p => p.id !== this.product.id);
      },
      error: () => {}
    });
  }

  getReviews() {
    this.productService.getReviewsByProduct(this.productId).subscribe((reviews) => {
      this.reviews = reviews;
    });
  }

  toggleReviewForm(event: Event): void {
    event.preventDefault();
    this.showReviewForm = true;
    this.cdr.detectChanges();
    setTimeout(() => {
      this.reviewFormElement?.nativeElement?.scrollIntoView({ behavior: 'smooth' });
    }, 100);
  }

  toggleReviewDisplay(event: Event): void {
    event.preventDefault();
    this.showReviews = !this.showReviews;
    if (this.showReviews) {
      setTimeout(() => {
        const el = document.getElementById('reviewsSection');
        if (el) el.scrollIntoView({ behavior: 'smooth' });
      }, 0);
    }
  }

  toggle(event: Event): void {
    event.preventDefault();
    this.showSimilarProducts = true;
    setTimeout(() => {
      this.topElement?.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }, 100);
  }

  submitReview() {
    if (!this.newReview.userName || !this.newReview.rating || !this.newReview.text) {
      this.toastMessage = 'Please fill out all fields before submitting your review.';
      this.toastRedirect = 'error';
      this.showToastMessage = true;

      setTimeout(() => {
        this.showToastMessage = false;
      }, 3000);
      return;
    }

    this.newReview.rating = Number(this.newReview.rating);
    if (isNaN(this.newReview.rating) || this.newReview.rating < 1 || this.newReview.rating > 5) {
      this.showError = true;
      return;
    }

    if (!this.newReview.productId && this.product?.id) {
      this.newReview.productId = this.product.id;
    }

    this.productService.addReview(this.newReview).subscribe({
      next: () => {
        this.newReview = {
          userName: '',
          text: '',
          rating: 5,
          productId: this.product.id
        };

        this.showError = false;
        this.getReviews();
        this.toastMessage = 'Review successfully added!';
        this.toastRedirect = 'success';
        this.showToastMessage = true;

        setTimeout(() => {
          this.showToastMessage = false;
        }, 3000);
      },
      error: () => {
        this.showError = true;
        this.toastMessage = 'Error adding review.';
        this.showToastMessage = true;

        setTimeout(() => {
          this.showToastMessage = false;
        }, 3000);
      }
    });
  }

  successMessage = '';

  addToCart(product: Product) {
    const quantityToAdd = this.quantity;
    const productToAdd = { ...product, quantity: quantityToAdd };

    this.cartService.addToCart(productToAdd).subscribe({
      next: () => {
        this.showToast(`${product.name} has been added to cart. Quantity: ${quantityToAdd}.`, 'cart');
      },
      error: (err) => {
        this.showToast(err?.error || 'An error occurred while adding the product to cart.', null);
      }
    });
  }

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

  navigateToCart() {
    this.router.navigate(['/public/cart']);
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/public/product', productId]);
  }

  closeImageZoom(): void {
    this.showImageZoom = false;
  }

  downloadProductImage(): void {
    this.productService.downloadProductImage(this.product.id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `${this.product.name.replace(/[^a-z0-9-_]/gi, '_')}-image`;
        link.click();
        URL.revokeObjectURL(url);
      },
      error: () => this.showToast('Image download failed.')
    });
  }
}
