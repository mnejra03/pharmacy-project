import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface BrandImageItem {
  id: number;
  name: string;
  logoUrl?: string;
}

interface AdvertisementImageItem {
  id: number;
  title: string;
  imageURL: string;
}

@Component({
  selector: 'app-admin-media',
  standalone: false,
  templateUrl: './media.component.html',
  styleUrls: ['./media.component.css']
})
export class MediaComponent implements OnInit {
  private readonly api = 'https://localhost:7057/api';
  brands: BrandImageItem[] = [];
  advertisements: AdvertisementImageItem[] = [];
  brandFiles: Record<number, File | null> = {};
  advertisementFiles: Record<number, File | null> = {};
  savingBrandId: number | null = null;
  savingAdvertisementId: number | null = null;
  errorMessage = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<BrandImageItem[]>(`${this.api}/products/brands/products/brands`).subscribe({
      next: items => this.brands = items,
      error: () => this.errorMessage = 'Nije moguće učitati brendove.'
    });
    this.http.get<AdvertisementImageItem[]>(`${this.api}/GetAdvertisementEndpoint`).subscribe({
      next: items => this.advertisements = items,
      error: () => this.errorMessage = 'Nije moguće učitati reklame.'
    });
  }

  selectBrandFile(id: number, event: Event): void {
    this.brandFiles[id] = (event.target as HTMLInputElement).files?.[0] ?? null;
  }

  selectAdvertisementFile(id: number, event: Event): void {
    this.advertisementFiles[id] = (event.target as HTMLInputElement).files?.[0] ?? null;
  }

  saveBrandLogo(brand: BrandImageItem): void {
    const file = this.brandFiles[brand.id];
    if (!file) return;
    const form = new FormData();
    form.append('file', file, file.name);
    this.savingBrandId = brand.id;
    this.http.put<BrandImageItem>(`${this.api}/brands/${brand.id}/logo`, form).subscribe({
      next: result => {
        brand.logoUrl = result.logoUrl;
        this.brandFiles[brand.id] = null;
        this.savingBrandId = null;
      },
      error: err => {
        this.errorMessage = typeof err?.error === 'string' ? err.error : 'Greška pri uploadu logotipa.';
        this.savingBrandId = null;
      }
    });
  }

  saveAdvertisementImage(advertisement: AdvertisementImageItem): void {
    const file = this.advertisementFiles[advertisement.id];
    if (!file) return;
    const form = new FormData();
    form.append('file', file, file.name);
    this.savingAdvertisementId = advertisement.id;
    this.http.put<AdvertisementImageItem>(`${this.api}/advertisements/${advertisement.id}/image`, form).subscribe({
      next: result => {
        advertisement.imageURL = result.imageURL;
        this.advertisementFiles[advertisement.id] = null;
        this.savingAdvertisementId = null;
      },
      error: err => {
        this.errorMessage = typeof err?.error === 'string' ? err.error : 'Greška pri uploadu reklame.';
        this.savingAdvertisementId = null;
      }
    });
  }
}
