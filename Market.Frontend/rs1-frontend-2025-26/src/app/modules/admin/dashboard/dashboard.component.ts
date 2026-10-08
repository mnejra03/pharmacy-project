import { Component, OnInit, inject } from '@angular/core';
import { DashboardApiService, DashboardStats } from '../../../api-services/dashboard/dashboard-api.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  template: `
    <section class="page" aria-labelledby="dashboard-title">
      <h1 id="dashboard-title">Pregled apoteke</h1>
      <div class="stats" *ngIf="stats">
        <mat-card>
          <mat-card-title>Korisnici</mat-card-title>
          <mat-card-content>{{ stats.users }}</mat-card-content>
        </mat-card>
        <mat-card>
          <mat-card-title>Farmaceuti</mat-card-title>
          <mat-card-content>{{ stats.pharmacists }}</mat-card-content>
        </mat-card>
        <mat-card>
          <mat-card-title>Recepti</mat-card-title>
          <mat-card-content>{{ stats.recipes }}</mat-card-content>
        </mat-card>
        <mat-card>
          <mat-card-title>Recepti na čekanju</mat-card-title>
          <mat-card-content>{{ stats.pendingRecipes }}</mat-card-content>
        </mat-card>
        <mat-card>
          <mat-card-title>Nepročitane obavijesti</mat-card-title>
          <mat-card-content>{{ stats.unreadNotifications }}</mat-card-content>
        </mat-card>
      </div>
    </section>
  `,
  styles: [`
    :host { display: block; min-height: 100%; color: #172b24; }
    .page { box-sizing: border-box; width: 100%; min-height: 100vh; padding: clamp(1.5rem, 4vw, 3rem); background: #f4f7f5; }
    h1 { margin: 0 0 1.75rem; color: #173e27; font-size: clamp(1.75rem, 3vw, 2.25rem); line-height: 1.25; font-weight: 700; }
    .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 220px), 1fr)); gap: 1.25rem; }
    mat-card { min-height: 150px; box-sizing: border-box; padding: 1.5rem; border: 1px solid #dce6df; border-radius: 12px; background: #fff; color: #172b24; box-shadow: 0 4px 14px rgb(22 55 34 / 8%); }
    mat-card-title { display: block; color: #34483b; font-size: 1rem; font-weight: 600; line-height: 1.5; }
    mat-card-content { margin-top: 1rem; color: #176b36; font-size: 2.5rem; font-weight: 700; line-height: 1.1; }
    @media (max-width: 600px) { .page { padding: 1.25rem; } .stats { grid-template-columns: 1fr; gap: 0.875rem; } mat-card { min-height: auto; padding: 1.25rem; } }
  `],
})
export class DashboardComponent implements OnInit {
  private api = inject(DashboardApiService);
  stats?: DashboardStats;

  ngOnInit(): void {
    this.api.getStats().subscribe((stats) => (this.stats = stats));
  }
}
