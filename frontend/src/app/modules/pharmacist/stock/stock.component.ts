import {Component, OnInit} from '@angular/core';
import {StockService} from '../../../services/stock.service';

@Component({
  selector: 'app-stock',
  standalone: false,
  templateUrl: './stock.component.html',
  styleUrl: './stock.component.css'
})
export class StockComponent implements OnInit {
  medicines: any[] = [];

  constructor(private stockService: StockService) {}

  ngOnInit(): void {
    this.loadMedicines();
  }

  loadMedicines(): void {
    this.stockService.getMedicines().subscribe(data => {
      this.medicines = data;
    });
  }

  orderMedicine(medicineId: number): void {
    const quantityString = prompt('Enter quantity:');
    const quantity = Number(quantityString);

    if (!quantity || quantity <= 0) {
      alert('Please enter a valid quantity greater than 0.');
      return;
    }

    this.stockService.orderMedicine(medicineId, quantity).subscribe({
      next: () => {
        alert('Order for medicine successfully created!');
        this.loadMedicines();
      },
      error: (error) => {
        console.error('Error when ordering:', error);
        alert('An error occurred while creating the order.');
      }
    });
  }

}
