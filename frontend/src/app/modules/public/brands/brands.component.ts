import { Component, ElementRef, ViewChild } from '@angular/core';
import { BrandService } from '../../../services/brand.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

interface Brand {
  id: number;
  name: string;
  logoUrl: string;
  description: string;
}

@Component({
  selector: 'app-brands',
  standalone: false,
  templateUrl: './brands.component.html',
  styleUrl: './brands.component.css'
})
export class BrandsComponent {
  brands: Brand[] = [];

  constructor(
    private brandService: BrandService,
    private router: Router,
    private http: HttpClient
  ) {
    this.loadBrands();
  }

  @ViewChild('brandsList', { static: false }) brandsList!: ElementRef;

  scrollLeft() {
    this.brandsList.nativeElement.scrollBy({ left: -300, behavior: 'smooth' });
  }

  scrollRight() {
    this.brandsList.nativeElement.scrollBy({ left: 300, behavior: 'smooth' });
  }

  loadBrands() {
    this.brandService.getBrands().subscribe(
      (data) => {
        this.brands = data;
      },
      (error) => {
        console.error('Greska pri ucitavanju brendova:', error);
      }
    );
  }
}
