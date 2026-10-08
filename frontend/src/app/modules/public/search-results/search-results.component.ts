import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ProductServicesService} from '../../../services/product-services.service';

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  quantityInStock: number;
  picture?: string;
  isDiscounted: boolean;
  discountPercentage: number;
  discountedPrice?: number;
}


@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  standalone: false,
  styleUrl: './search-results.component.css'
})
export class SearchResultsComponent implements OnInit {

  product: Product | null = null;



  searchQuery: string = '';
  searchResults: Product[] = [];

  constructor(
    private productService: ProductServicesService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const query = params['q'];
      if (query) {
        this.searchQuery = query;
        this.searchProducts(query);
      }
    });
  }

  searchProducts(query: string): void {
    this.productService.searchProducts(query).subscribe({
      next: (products) => {
        this.searchResults = products;
      },
      error: (err) => {
        console.error('Greška prilikom učitavanja proizvoda:', err);
      }
    });

  }

}
