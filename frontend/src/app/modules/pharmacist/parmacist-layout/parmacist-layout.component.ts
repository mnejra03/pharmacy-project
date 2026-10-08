import { Component } from '@angular/core';
import {Router} from '@angular/router';
import {AuthLogoutEndpointService} from '../../../endpoints/auth-endpoints/auth-logout-endpoint.service';

@Component({
  selector: 'app-parmacist-layout',
  standalone: false,
  templateUrl: './parmacist-layout.component.html',
  styleUrl: './parmacist-layout.component.css'
})
export class ParmacistLayoutComponent {
  currentPharmacist: any;
  constructor(private router: Router,private logoutService: AuthLogoutEndpointService) {

    const user = localStorage.getItem('pharmacist');
    if (user) {
      this.currentPharmacist = JSON.parse(user);
    }
  }

  showLogoutModal: boolean = false;
  logout() {
    this.showLogoutModal = true;
  }

  confirmLogout() {
    this.showLogoutModal = false;
    this.logoutService.handleAsync().subscribe({
      next: () => {},
      error: () => {}
    });
  }

  cancelLogout() {
    this.showLogoutModal = false;
  }

}
