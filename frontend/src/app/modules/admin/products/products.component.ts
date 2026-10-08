import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProductServicesService } from '../../../services/product-services.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Product } from '../../../services/product-services.service';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import {MyDialogConfirmComponent} from '../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import {MatDialog} from '@angular/material/dialog';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  brands: any[] = [];
  categories: any[] = [];
  productForm: FormGroup;
  editing = false;
  isLoading = false;
  selectedImage: File | null = null;
  imagePreviewUrl: string | null = null;


  searchName = '';
  selectedCategory: number | null = null;
  selectedBrand: number | null = null;
  minPrice: number | null = null;
  onlyDiscounted = false;


  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  private filterSubject = new Subject<void>();
  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductServicesService,
    private fb: FormBuilder,
    private dialog: MatDialog
  ) {
    this.productForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)],
      price: [null, [Validators.required, Validators.min(0.01)]],
      quantityInStock: [0, [Validators.required, Validators.min(0)]],
      picture: [''],
      categoryId: [null, Validators.required],
      brandId: [null],
      isDiscounted: [false],
      discountPercentage: [null, [Validators.min(1), Validators.max(100)]]
    });
  }

  ngOnInit(): void {
    this.loadBrands();
    this.loadCategories();


    this.filterSubject.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.currentPage = 1;
      this.loadProducts();
    });

    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProducts(): void {
    this.isLoading = true;

    this.productService.getProductsPaged(
      this.searchName,
      this.selectedCategory,
      this.selectedBrand,
      this.minPrice,
      this.onlyDiscounted,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (result) => {
        this.products = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    this.filterSubject.next();
  }

  resetFilter(): void {
    this.searchName = '';
    this.selectedCategory = null;
    this.selectedBrand = null;
    this.minPrice = null;
    this.onlyDiscounted = false;
    this.currentPage = 1;
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadProducts();
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    const product = this.productForm.value;

    if (!product.picture && !this.selectedImage) {
      alert('Unesite URL slike ili odaberite sliku za upload.');
      return;
    }

    if (product.isDiscounted && (!product.discountPercentage || product.discountPercentage <= 0)) {
      alert('Please enter a valid discount percentage.');
      return;
    }

    if (this.selectedImage) {
      this.productService.uploadProductImage(this.selectedImage).subscribe({
        next: ({ imageUrl }) => {
          product.picture = imageUrl;
          this.saveProduct(product);
        },
        error: (err) => alert('Error uploading image: ' + JSON.stringify(err.error))
      });
      return;
    }

    this.saveProduct(product);
  }

  private saveProduct(product: Product): void {
    if (this.editing) {
      this.productService.updateProduct(product).subscribe({
        next: () => {
          this.loadProducts();
          this.productForm.reset({ id: 0, isDiscounted: false });
          this.editing = false;
          this.clearSelectedImage();
        },
        error: (err) => alert('Error updating product: ' + JSON.stringify(err.error))
      });
    } else {
      this.productService.addProduct(product).subscribe({
        next: () => {
          this.loadProducts();
          this.productForm.reset({ id: 0, isDiscounted: false });
          this.clearSelectedImage();
        },
        error: (err) => alert('Error adding product: ' + JSON.stringify(err.error))
      });
    }
  }

  onEdit(product: Product): void {
    this.clearSelectedImage();
    this.productForm.patchValue(product);
    this.editing = true;
  }

  onDelete(id: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete product',
        message: 'Are you sure you want to delete this product? This action cannot be undone.',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.productService.deleteProduct(id).subscribe({
          next: () => this.loadProducts(),
          error: (err) => console.error('Error deleting product:', err)
        });
      }
    });
  }

  cancelEdit(): void {
    this.productForm.reset({ id: 0, isDiscounted: false });
    this.editing = false;
    this.clearSelectedImage();
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedImage = input.files?.[0] ?? null;
    this.imagePreviewUrl = this.selectedImage ? URL.createObjectURL(this.selectedImage) : null;
  }

  private clearSelectedImage(): void {
    if (this.imagePreviewUrl) URL.revokeObjectURL(this.imagePreviewUrl);
    this.selectedImage = null;
    this.imagePreviewUrl = null;
  }

  loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: (data) => this.brands = data,
      error: (err) => console.error(err)
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (data) => this.categories = data,
      error: (err) => console.error(err)
    });
  }
}
