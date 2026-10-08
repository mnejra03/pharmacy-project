import { ChangeDetectorRef, Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SearchService } from '../../../services/search.service';
import { ProductServicesService, Product } from '../../../services/product-services.service';
import { WishlistService } from '../../../services/wishlist.service';
import { AuthLogoutEndpointService } from '../../../endpoints/auth-endpoints/auth-logout-endpoint.service';
import { RecipeService } from '../../../services/recipe.service';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';
import { CartService } from '../../../services/cart.service';

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent implements OnInit {
  constructor(
    private router: Router,
    private searchService: SearchService,
    private productService: ProductServicesService,
    private logoutService: AuthLogoutEndpointService,
    private recipeService: RecipeService,
    private authService: MyAuthService,
    private cartService: CartService,
    private wishListService: WishlistService,
    private cdr: ChangeDetectorRef
  ) {}

  wishListCount = 0;
  userId: number | null = null;

  ngOnInit() {
    this.wishListService.wishListCount$.subscribe(count => {
      this.wishListCount = count;
    });

    if (this.authService.isLoggedIn() && this.authService.isCustomer()) {
      this.wishListService.loadWishList().subscribe({
        error: (err) => {
          console.error('Greska prilikom ucitavanja wishlist-e:', err);
        }
      });
    }

    this.cartService.loadCart().subscribe({
      error: (err) => {
        console.error('Greska prilikom ucitavanja korpe:', err);
      }
    });

    const tokenString = localStorage.getItem('my-auth-token');
    if (tokenString) {
      try {
        const token = JSON.parse(tokenString);
        this.userId = token?.myAuthInfo?.userId || null;
      } catch (e) {
        console.error('Greska pri parsiranju tokena:', e);
      }
    }
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    const navigation = document.querySelector('.navigation');
    if (window.scrollY > 100) {
      navigation?.classList.add('sticky');
    } else {
      navigation?.classList.remove('sticky');
    }
  }

  searchQuery = '';
  searchResults: Product[] = [];

  onSearch(): void {
    const query = this.searchQuery.toLowerCase().trim();

    if (!query) {
      this.searchResults = [];
      return;
    }

    this.productService.searchProducts(query).subscribe({
      next: (products) => {
        this.searchResults = products.filter(product =>
          product.name.toLowerCase().includes(query)
        );
      },
      error: (err) => {
        console.error('Greska prilikom ucitavanja proizvoda:', err);
      }
    });
  }

  goToProduct(productId: number): void {
    this.router.navigate(['/public/product', productId]);
    this.searchResults = [];
    this.searchQuery = '';
  }

  isCustomerLoggedIn(): boolean {
    const tokenString = localStorage.getItem('my-auth-token');
    if (!tokenString) return false;
    try {
      const token = JSON.parse(tokenString);
      return !!token?.myAuthInfo?.isCustomer;
    } catch (e) {
      console.error('Neispravan token format:', e);
      return false;
    }
  }

  showLogoutModal = false;

  logout() {
    this.showLogoutModal = true;
    this.cdr.detectChanges();
  }

  confirmLogout(): void {
    this.logoutService.handleAsync().subscribe({
      next: () => {},
      error: (err) => {
        console.error('Unexpected logout error:', err);
        this.authService.setLoggedInUser(null);
        this.router.navigate(['/auth/login']);
      }
    });

    this.showLogoutModal = false;
  }

  cancelLogout() {
    this.showLogoutModal = false;
  }

  showRecipeModal = false;
  recipes: any[] = [];

  openRecipes() {
    const tokenString = localStorage.getItem('my-auth-token');
    if (!tokenString) {
      return;
    }

    try {
      const token = JSON.parse(tokenString);
      const authInfo = token?.myAuthInfo;

      if (!authInfo?.isCustomer) {
        return;
      }

      const userId = authInfo.userId;

      if (!userId) {
        return;
      }

      this.recipeService.getMyRecipes().subscribe({
        next: (res) => {
          this.recipes = res;
          this.showRecipeModal = true;
        },
        error: (err) => {
          console.error('Greska prilikom ucitavanja recepata:', err);
        }
      });
    } catch (e) {
      console.error('Greska prilikom parsiranja tokena:', e);
    }
  }

  getLoggedUserId(): number | null {
    const tokenString = localStorage.getItem('my-auth-token');
    if (!tokenString) return null;

    try {
      const token = JSON.parse(tokenString);
      return token?.myAuthInfo?.userId || null;
    } catch (e) {
      console.error('Neispravan token format:', e);
      return null;
    }
  }

  closeRecipeModal() {
    this.showRecipeModal = false;
  }

  showAddForm = false;
  newRecipe = {
    doctorFirstname: '',
    doctorLastname: '',
  };
  selectedFile: File | null = null;
  previewUrl: string | null = null;

  toggleAddRecipeForm() {
    this.showAddForm = !this.showAddForm;
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
    if (this.selectedFile) {
      const reader = new FileReader();
      reader.onload = (e: any) => this.previewUrl = e.target.result;
      reader.readAsDataURL(this.selectedFile);
    }
  }

  addRecipe() {
    if (
      !this.newRecipe.doctorFirstname ||
      !this.newRecipe.doctorLastname ||
      !this.selectedFile
    ) {
      this.errorMessage = 'Please fill in all fields and select an image.';
      this.successMessage = null;
      return;
    }

    this.errorMessage = null;

    const userId = this.getLoggedUserId();
    if (!userId) {
      this.errorMessage = 'User is not logged in.';
      return;
    }

    const formData = new FormData();
    formData.append('doctorFirstname', this.newRecipe.doctorFirstname);
    formData.append('doctorLastname', this.newRecipe.doctorLastname);
    formData.append('scan', this.selectedFile);

    this.recipeService.addRecipe(formData).subscribe({
      next: (data) => {
        this.recipes = data;
        this.resetForm();

        this.successMessage = 'Recipe added successfully!';
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        console.error('Error adding recipe', err);
        this.errorMessage = 'An error occurred while adding the recipe.';
      }
    });
  }

  resetForm() {
    this.newRecipe = { doctorFirstname: '', doctorLastname: '' };
    this.selectedFile = null;
    this.previewUrl = null;
    this.showAddForm = false;
  }

  errorMessage: string | null = null;
  successMessage: string | null = null;
}
