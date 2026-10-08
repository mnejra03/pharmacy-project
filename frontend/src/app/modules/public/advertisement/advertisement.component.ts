import { Component, OnInit, OnDestroy } from '@angular/core';
import { AdvertisementService, Advertisement} from '../../../services/advertisement.service';

@Component({
  selector: 'app-advertisement',
  templateUrl: './advertisement.component.html',
  styleUrls: ['./advertisement.component.css'],
  standalone: false
})
export class AdvertisementComponent implements OnInit, OnDestroy {

  advertisements: Advertisement[] = [];
  currentAdIndex = 0;
  currentAdvertisement?: Advertisement;
  intervalId: any;

  constructor(private advertisementService: AdvertisementService) {}

  ngOnInit() {
    this.loadAdvertisements();
  }

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  loadAdvertisements() {
    this.advertisementService.getAdvertisements().subscribe({
      next: (data) => {
        this.advertisements = data;

        if (this.advertisements.length > 0) {
          this.updateCurrentAdvertisement();
          this.startSlideshow();
        }
      },
      error: (err) => {
        console.error('Error fetching ads:', err);
      }
    });
  }

  startSlideshow() {
    this.intervalId = setInterval(() => {
      this.nextAdvertisement();
    }, 3500);
  }

  nextAdvertisement() {
    this.currentAdIndex =
      (this.currentAdIndex + 1) % this.advertisements.length;
    this.updateCurrentAdvertisement();
  }

  updateCurrentAdvertisement() {
    this.currentAdvertisement = this.advertisements[this.currentAdIndex];
  }

  onImageError() {
  }
}
