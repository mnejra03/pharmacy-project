import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  quantityInStock: number;
  picture?: string;
  categoryId?: number;
  isDiscounted: boolean;
  discountPercentage: number;
  discountedPrice?: number;
  brandId?: number;
  imageUrl?: string;
  reviews: Review[];
}

export interface Review {
  id?: number;
  userName: string;
  text: string;
  rating: number;
  productId: number;
}


@Injectable({
  providedIn: 'root'
})
export class ProductServicesService {
  private baseUrl = 'https://localhost:7057/api';

  constructor(private http: HttpClient) {}

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/GetProductsByCategory/${categoryId}`);
  }

  searchProducts(query: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/products/search`, {
      params: { query }
    });
  }

  getProductById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/products/${id}`);
  }

  addReview(review: any) {
    return this.http.post('https://localhost:7057/api/add-review', review, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  getReviewsByProduct(productId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`https://localhost:7057/api/${productId}`);
  }

  getSimilarProducts(keyword: string, excludeProductId: number): Observable<Product[]> {
    const url = `${this.baseUrl}/products/similar?keyword=${keyword}&excludeId=${excludeProductId}`;
    return this.http.get<Product[]>(url);
  }

  getProductsByBrand(brandId: number): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7057/api/by-brand/${brandId}`);
  }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`https://localhost:7057/api/GetProductEndpoint`);
  }

  addProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}/PostProductEndpoint`, product);
  }

  uploadProductImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<{ imageUrl: string }>(`${this.baseUrl}/images/products`, formData);
  }

  updateProduct(product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/PutProductEndpoint/${product.id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/DeleteProductEndpoint/${id}`);
  }

  getBrands(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7057/api/products/brands/products/brands`);
  }

  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7057/api/getCategory`);
  }

  getProductsPaged(
    name?: string,
    categoryId?: number | null,
    brandId?: number | null,
    minPrice?: number | null,
    onlyDiscounted?: boolean,
    page: number = 1,
    pageSize: number = 10
  ): Observable<{ items: Product[], totalCount: number, page: number, pageSize: number, totalPages: number }> {
    let params: any = { page, pageSize };

    if (name) params.name = name;
    if (categoryId) params.categoryId = categoryId;
    if (brandId) params.brandId = brandId;
    if (minPrice) params.minPrice = minPrice;
    if (onlyDiscounted) params.onlyDiscounted = true;

    return this.http.get<any>(`${this.baseUrl}/GetProductsPagedEndpoint`, { params });
  }
}

