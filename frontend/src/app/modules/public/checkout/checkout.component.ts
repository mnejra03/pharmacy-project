import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { CartService } from '../../../services/cart.service';
import {
  CreatePaymentIntentDTO,
  OrderService,
  OrderCreateDTO
} from '../../../services/order.service';
import { Router } from '@angular/router';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '../../../services/notification.service';
import { firstValueFrom } from 'rxjs';

declare const Stripe: any;

@Component({
  selector: 'app-checkout',
  standalone: false,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements AfterViewInit, OnDestroy {
  userForm!: FormGroup;
  isModalVisible = false;

  cart: any[] = [];
  totalPrice = 0;
  orderId!: string;
  today: Date = new Date();

  currentStageIndex = 0;
  stages: any[] = [];
  deliveryMethod = '';
  paymentMethod = '';
  stripe: any = null;
  stripeElements: any = null;
  stripeCardElement: any = null;
  paymentClientSecret: string | null = null;
  paymentToken: string | null = null;
  stripePublishableKey: string | null = null;
  isPreparingPayment = false;
  paymentErrorMessage = '';

  showError = false;
  errorMessage = '';
  showDeliveryError = false;
  showPaymentError = false;

  @ViewChildren('formField') formFields!: QueryList<ElementRef>;
  @ViewChild('cardElementContainer') cardElementContainer?: ElementRef<HTMLDivElement>;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router,
    private authService: MyAuthService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      localStorage.setItem('redirectAfterLogin', '/public/checkout');
      this.router.navigate(['/auth/login']);
      return;
    }

    if (!this.authService.isCustomer()) {
      alert('Only customers can access checkout.');
      this.router.navigate(['/public']);
      return;
    }

    this.cartService.loadCart().subscribe({
      next: (items) => {
        this.cart = items;

        if (this.cart.length === 0) {
          alert('Your cart is empty!');
          this.router.navigate(['/public']);
          return;
        }

        this.calculateTotalPrice(this.deliveryMethod === 'express' ? 10 : this.deliveryMethod === 'standard' ? 6 : 0);
      },
      error: () => {
        alert('Unable to load your cart.');
        this.router.navigate(['/public/cart']);
      }
    });

    this.orderService.getTrackingStages().subscribe((data) => {
      this.stages = data;
    });

    this.today = new Date();
    this.generateOrderId();

    const authInfo = this.authService.getMyAuthInfo();

    this.userForm = this.fb.group({
      firstName: [authInfo?.firstName || '', Validators.required],
      lastName: [authInfo?.lastName || '', Validators.required],
      email: [authInfo?.email || '', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^(\+)?[0-9]{6,15}$/)]],
      address: ['', Validators.required],
      city: ['', Validators.required],
      zip: ['', [Validators.required, Validators.pattern(/^\d{4,6}$/)]],
      country: ['', Validators.required],
    });
  }

  ngAfterViewInit(): void {
    if (this.paymentMethod === 'card') {
      void this.initializeStripeCardElement();
    }
  }

  ngOnDestroy(): void {
    this.destroyStripeCardElement();
  }

  async onSubmitOrder(): Promise<void> {
    const userId = this.authService.getCurrentUserId();
    if (!userId) {
      alert('User not authenticated. Please login again.');
      this.router.navigate(['/auth/login']);
      return;
    }

    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      this.isModalVisible = true;
      return;
    }

    let paymentToken: string | undefined;

    if (this.paymentMethod === 'card') {
      try {
        paymentToken = await this.confirmStripePayment();
      } catch (error: any) {
        this.paymentErrorMessage = error?.message || 'Card payment could not be completed.';
        this.showPaymentError = true;
        return;
      }
    }

    const checkoutOrder: OrderCreateDTO = {
      address: this.userForm.value.address,
      city: this.userForm.value.city,
      postalCode: this.userForm.value.zip,
      country: this.userForm.value.country,
      paymentMethod: this.paymentMethod,
      paymentToken,
      deliveryMethod: this.deliveryMethod,
      items: this.cart.map(item => ({
        productId: item.id,
        qty: item.quantity
      }))
    };

    try {
      await firstValueFrom(this.orderService.postOrder(checkoutOrder));
      alert('Order placed successfully!');
      this.cartService.clearCart();
      this.notificationService.notifyNotificationsChanged();
      this.router.navigate(['/public']);
    } catch (err: any) {
      if (err.status === 400) {
        alert(err.error || 'Invalid order data.');
      } else if (err.status === 401) {
        alert('Session expired. Please login again.');
        this.router.navigate(['/auth/login']);
      } else {
        alert('An error occurred while placing the order.');
      }
    }
  }

  generateOrderId() {
    const random = Math.floor(100000 + Math.random() * 900000);
    const timestamp = Date.now().toString().slice(-4);
    this.orderId = `#${random}${timestamp}`;
  }

  goToNextStage() {
    if (this.currentStageIndex === 1) {
      if (this.userForm.invalid) {
        this.userForm.markAllAsTouched();
        this.errorMessage = 'Please fill in all required user data fields correctly.';
        this.showError = true;
        return;
      }
    }

    if (this.currentStageIndex === 3) {
      if (this.paymentMethod === 'card') {
        if (!this.paymentClientSecret || !this.stripeCardElement) {
          this.showPaymentError = true;
          this.paymentErrorMessage = 'Payment form is not ready yet.';
          return;
        }
      }
    }

    this.showError = false;
    this.errorMessage = '';
    this.currentStageIndex++;
  }

  goToPreviousStage() {
    if (this.currentStageIndex > 0) this.currentStageIndex--;
  }

  onDeliveryMethodChange(): void {
    let deliveryCost = 0;

    if (this.deliveryMethod === 'express') {
      deliveryCost = 10;
    } else if (this.deliveryMethod === 'standard') {
      deliveryCost = 6;
    }

    this.calculateTotalPrice(deliveryCost);

    if (this.paymentMethod === 'card') {
      this.clearCardDetails();
      this.scheduleStripeInitialization();
    }
  }

  calculateTotalPrice(deliveryCost: number): void {
    const productsTotal = this.cart.reduce((acc, item) => acc + (item.price * item.quantity), 0);
    this.totalPrice = productsTotal + deliveryCost;
  }

  onPaymentMethodChange(): void {
    if (this.paymentMethod !== 'card') {
      this.clearCardDetails();
      return;
    }

    this.scheduleStripeInitialization();
  }

  clearCardDetails(): void {
    this.destroyStripeCardElement();
    this.paymentClientSecret = null;
    this.paymentToken = null;
    this.paymentErrorMessage = '';
  }

  private async initializeStripeCardElement(): Promise<void> {
    if (!this.authService.getCurrentUserId() || !this.deliveryMethod) {
      return;
    }

    if (!this.cardElementContainer?.nativeElement) {
      return;
    }

    this.isPreparingPayment = true;
    this.paymentErrorMessage = '';

    try {
      const request: CreatePaymentIntentDTO = {
        deliveryMethod: this.deliveryMethod,
        items: this.cart.map(item => ({
          productId: item.id,
          qty: item.quantity
        }))
      };

      const response = await firstValueFrom(this.orderService.createPaymentIntent(request));

      this.paymentClientSecret = response.clientSecret;
      this.paymentToken = response.paymentIntentId;
      this.stripePublishableKey = response.publishableKey;

      if (typeof Stripe === 'undefined') {
        throw new Error('Stripe.js is not loaded.');
      }

      if (!this.stripe || this.stripePublishableKey !== response.publishableKey) {
        this.stripe = Stripe(response.publishableKey);
      }

      this.destroyStripeCardElement();
      this.stripeElements = this.stripe.elements();
      this.stripeCardElement = this.stripeElements.create('card', {
        hidePostalCode: true
      });

      this.stripeCardElement.mount(this.cardElementContainer.nativeElement);
      this.stripeCardElement.on('change', (event: any) => {
        this.paymentErrorMessage = event.error?.message ?? '';
        this.showPaymentError = !!event.error;
        this.cdr.detectChanges();
      });
    } catch (error: any) {
      this.clearCardDetails();
      this.paymentErrorMessage = error?.error || error?.message || 'Unable to prepare secure payment.';
      this.showPaymentError = true;
    } finally {
      this.isPreparingPayment = false;
      this.cdr.detectChanges();
    }
  }

  private scheduleStripeInitialization(): void {
    this.cdr.detectChanges();

    setTimeout(() => {
      void this.initializeStripeCardElement();
    });
  }

  private destroyStripeCardElement(): void {
    if (this.stripeCardElement) {
      this.stripeCardElement.unmount();
      this.stripeCardElement.destroy();
      this.stripeCardElement = null;
    }

    this.stripeElements = null;
  }

  private async confirmStripePayment(): Promise<string> {
    if (!this.paymentClientSecret || !this.stripe || !this.stripeCardElement) {
      await this.initializeStripeCardElement();
    }

    if (!this.paymentClientSecret || !this.stripe || !this.stripeCardElement) {
      throw new Error('Payment form is not ready.');
    }

    const result = await this.stripe.confirmCardPayment(this.paymentClientSecret, {
      payment_method: {
        card: this.stripeCardElement,
        billing_details: {
          name: `${this.userForm.value.firstName} ${this.userForm.value.lastName}`,
          email: this.userForm.value.email
        }
      }
    });

    if (result.error) {
      throw new Error(result.error.message);
    }

    if (!result.paymentIntent || result.paymentIntent.status !== 'succeeded') {
      throw new Error('Payment was not completed.');
    }

    return result.paymentIntent.id;
  }

  handleNextClick() {
    if (!this.deliveryMethod) {
      this.showDeliveryError = true;
      return;
    }
    this.goToNextStage();
  }

  closeModal() {
    this.isModalVisible = false;
  }

  closeError() {
    this.showError = false;
  }

  closeDeliveryError() {
    this.showDeliveryError = false;
  }

  closePaymentError() {
    this.showPaymentError = false;
    this.paymentErrorMessage = '';
  }

  continueShopping(): void {
    this.router.navigate(['/public']);
  }
}
