import { Component, OnInit } from '@angular/core';
import { AdminDashboardService, DashboardStats } from '../../../services/admin-dashboard.service';
import { ChartConfiguration, ChartType } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: false
})
export class DashboardComponent implements OnInit {

  stats: DashboardStats | null = null;

  salesChartType: ChartType = 'line';

  salesChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: []
  };

  salesChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { display: true }
    },
    scales: {
      x: { type: 'category' },
      y: { beginAtZero: true }
    }
  };


  constructor(private dashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;

        this.salesChartData = {
          labels: data.dailySalesData.map(d => d.date),
          datasets: [
            {
              data: data.dailySalesData.map(d => d.amount),
              label: 'Daily sales',
              fill: true,
              tension: 0.4,
              borderColor: '#2e7d32',
              backgroundColor: 'rgba(46,125,50,0.2)',
              pointBackgroundColor: '#2e7d32'
            }
          ]
        };
      },
      error: err => console.error('Dashboard error:', err)
    });
  }

  protected readonly Date = Date;
}
