import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  isLoggedInCustomer = false;
  ngOnInit() {
    const auth = localStorage.getItem('my-auth-token');
    if (auth) {
      const info = JSON.parse(auth);
      this.isLoggedInCustomer = info.myAuthInfo?.isCustomer;
    }
  }
}
