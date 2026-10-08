import { Component, OnInit, inject } from '@angular/core';
import { Advertisement, AdvertisementsApiService } from '../../../api-services/advertisements/advertisements-api.service';

@Component({ selector: 'app-public-layout', standalone: false, templateUrl: './public-layout.component.html', styleUrl: './public-layout.component.scss' })
export class PublicLayoutComponent implements OnInit {
  private advertisementsApi = inject(AdvertisementsApiService);
  advertisements: Advertisement[] = [];
  activeIndex = 0;
  currentYear = new Date().getFullYear();
  ngOnInit(): void { this.advertisementsApi.getAll().subscribe({ next: items => this.advertisements = items, error: () => this.advertisements = [] }); }
  previous(): void { if (this.advertisements.length) this.activeIndex = (this.activeIndex - 1 + this.advertisements.length) % this.advertisements.length; }
  next(): void { if (this.advertisements.length) this.activeIndex = (this.activeIndex + 1) % this.advertisements.length; }
}
