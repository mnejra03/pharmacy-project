import { Component } from '@angular/core';
import { AuthLogoutEndpointService } from '../../../endpoints/auth-endpoints/auth-logout-endpoint.service';
import { MyAppUser } from '../../../services/admin-user.service';


@Component({
  selector: 'app-admin-layout',
  standalone: false,
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.css']
})
export class AdminLayoutComponent {
  adminUser!: MyAppUser;
  constructor(private logoutService: AuthLogoutEndpointService) {}

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

