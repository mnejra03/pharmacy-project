import { Component, OnInit, inject } from '@angular/core';
import { Advertisement, AdvertisementsApiService } from '../../../api-services/advertisements/advertisements-api.service';
import { Product, StoreApiService } from '../../../api-services/store/store-api.service';

@Component({
  selector: 'app-public-home',
  standalone: false,
  template: `
    <section class="home-hero">
      <div class="hero-copy">
        <p class="eyebrow">Vaša apoteka</p>
        <h1>Zdravlje i njega za cijelu porodicu</h1>
        <p>Pronađite proizvode, pratite narudžbe i obratite se farmaceutu na jednom mjestu.</p>
        <a routerLink="/catalog" class="shop-button">Pregledaj proizvode</a>
      </div>
      <div class="home-promo" *ngIf="advertisements.length">
        <button type="button" (click)="previous()" aria-label="Prethodni oglas">‹</button>
        <a routerLink="/catalog" class="promo-image">
          <img [src]="advertisements[activeIndex].imageUrl" [alt]="advertisements[activeIndex].title" (error)="promoFallback($event)" />
        </a>
        <button type="button" (click)="next()" aria-label="Sljedeći oglas">›</button>
        <span>{{ advertisements[activeIndex].title }}</span>
      </div>
      <div class="home-promo promo-placeholder" *ngIf="!advertisements.length">
        <img src="images/shipBanner.png" alt="Dostava proizvoda iz apoteke" />
      </div>
    </section>

    <section class="home-categories">
      <a routerLink="/catalog" [queryParams]="{ category: 'Your health' }">Zdravlje</a>
      <a routerLink="/catalog" [queryParams]="{ category: 'Beauty and care' }">Njega i ljepota</a>
      <a routerLink="/catalog" [queryParams]="{ category: 'Childcare' }">Djeca</a>
      <a routerLink="/catalog" [queryParams]="{ category: 'Skin protection' }">Zaštita kože</a>
      <a routerLink="/catalog" [queryParams]="{ category: 'Devices' }">Uređaji</a>
    </section>

    <section class="home-products" *ngIf="products.length">
      <div class="section-heading"><h2>Proizvodi za vas</h2><a routerLink="/catalog">Pogledaj sve</a></div>
      <div class="featured-grid">
        <a class="featured-card" *ngFor="let product of products" [routerLink]="['/product', product.id]">
          <img [src]="product.imageUrl" [alt]="product.name" (error)="imageFallback($event)" />
          <span>{{ product.name }}</span>
          <strong>{{ product.currentPrice | number:'1.2-2' }} KM</strong>
        </a>
      </div>
    </section>
  `,
  styles: [`
    :host { display:block; max-width:1440px; margin:0 auto; font-family:Poppins,Arial,sans-serif; }
    .home-hero { display:grid; grid-template-columns:1.1fr .9fr; gap:2rem; align-items:center; padding:2rem; background:#d0eccf; border-radius:12px; }
    .hero-copy { max-width:560px; } .eyebrow { color:#1b5e20; font-weight:700; text-transform:uppercase; letter-spacing:.08em; }
    h1 { margin:.5rem 0 1rem; color:#1b5e20; font-size:clamp(2rem,4vw,3rem); line-height:1.15; }
    .hero-copy>p:not(.eyebrow) { color:#344638; line-height:1.7; }
    .shop-button { display:inline-block; margin-top:1rem; padding:.8rem 1.3rem; border-radius:6px; background:#1b5e20; color:white; font-weight:700; text-decoration:none; }
    .home-promo { position:relative; display:flex; align-items:center; justify-content:center; gap:.5rem; min-height:250px; padding:1rem; background:white; border-radius:10px; }
    .promo-image { display:grid; place-items:center; width:100%; height:245px; } .promo-image img,.promo-placeholder img { max-width:100%; max-height:245px; object-fit:contain; }
    .home-promo button { flex:0 0 2.5rem; height:2.5rem; border:0; border-radius:50%; background:#1b5e20; color:white; font-size:1.6rem; cursor:pointer; }
    .home-promo span { position:absolute; bottom:.6rem; left:1rem; right:1rem; color:#1b5e20; text-align:center; font-weight:600; }
    .promo-placeholder img { width:100%; }
    .home-categories { display:grid; grid-template-columns:repeat(5,1fr); gap:.75rem; margin:1.5rem 0; }
    .home-categories a { padding:1rem .75rem; border:1px solid #c8e6c9; border-radius:8px; background:white; color:#1b5e20; text-align:center; font-weight:600; text-decoration:none; }
    .home-categories a:hover { background:#e8f5e9; }
    .section-heading { display:flex; justify-content:space-between; align-items:center; gap:1rem; margin:1.5rem 0 1rem; }
    .section-heading h2 { color:#1b5e20; } .section-heading a { color:#1b5e20; font-weight:600; }
    .featured-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(170px,1fr)); gap:1rem; }
    .featured-card { display:flex; flex-direction:column; gap:.5rem; padding:1rem; border:1px solid #e0e0e0; border-radius:8px; background:white; color:#333; text-decoration:none; }
    .featured-card img { width:100%; height:150px; object-fit:contain; } .featured-card strong { color:#1b5e20; }
    @media(max-width:760px) { .home-hero { grid-template-columns:1fr; padding:1rem; } .home-categories { grid-template-columns:repeat(2,1fr); } .home-categories a:last-child { grid-column:span 2; } }
  `]
})
export class HomeComponent implements OnInit {
  private readonly adsApi = inject(AdvertisementsApiService);
  private readonly store = inject(StoreApiService);
  advertisements: Advertisement[] = [];
  products: Product[] = [];
  activeIndex = 0;

  ngOnInit(): void {
    this.adsApi.getAll().subscribe({ next: items => this.advertisements = items, error: () => this.advertisements = [] });
    this.store.getProducts({ page: 1, pageSize: 6 }).subscribe({ next: page => this.products = page.items, error: () => this.products = [] });
  }

  previous(): void { if (this.advertisements.length) this.activeIndex = (this.activeIndex - 1 + this.advertisements.length) % this.advertisements.length; }
  next(): void { if (this.advertisements.length) this.activeIndex = (this.activeIndex + 1) % this.advertisements.length; }
  imageFallback(event: Event): void { (event.target as HTMLImageElement).src = '/images/product-placeholder.svg'; }
  promoFallback(event: Event): void { (event.target as HTMLImageElement).src = '/images/shipBanner.png'; }
}
