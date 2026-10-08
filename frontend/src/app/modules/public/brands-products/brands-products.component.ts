import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductServicesService } from '../../../services/product-services.service';
import { BrandService } from '../../../services/brand.service';

@Component({
  selector: 'app-brands-products',
  standalone: false,
  templateUrl: './brands-products.component.html',
  styleUrl: './brands-products.component.css'
})
export class BrandsProductComponent implements OnInit {
  brandId!: number;
  products: any[] = [];
  brand: any;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductServicesService,
    private brandService: BrandService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      this.brandId = id ? +id : NaN;

      if (!isNaN(this.brandId)) {
        this.loadProducts();
        this.getBrandDetails();
      } else {
        console.error('Brand ID nije ispravan!');
      }
    });
  }

  getBrandDetails() {
    this.brandService.getBrandById(this.brandId).subscribe({
      next: (data) => {
        this.brand = data;
      },
      error: (err) => {
        console.error('Error loading brand:', err);
      }
    });
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/public/product', productId]);
  }

  loadProducts() {
    if (this.brandId) {
      this.productService.getProductsByBrand(this.brandId).subscribe(
        (data) => {
          this.products = data;
        },
        (error) => {
          console.error('Error loading product:', error);
        }
      );
    } else {
      console.error('Brand ID is not correct!');
    }
  }
}
