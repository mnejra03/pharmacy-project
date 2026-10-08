import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthLoginEndpointService } from '../../../endpoints/auth-endpoints/auth-login-endpoint.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MyInputTextType } from '../../shared/my-reactive-forms/my-input-text/my-input-text.component';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';
import { ChatService } from '../../../services/chat.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false,
})
export class LoginComponent {
  form: FormGroup;
  loginError = false;
  protected readonly MyInputTextType = MyInputTextType;

  constructor(
    private fb: FormBuilder,
    private authLoginService: AuthLoginEndpointService,
    private authService: MyAuthService,
    private chatService: ChatService,
    private router: Router
  ) {
    this.form = this.fb.group({
      username: ['admin', [Validators.required, Validators.min(2), Validators.max(15)]],
      password: ['test', [Validators.required, Validators.min(2), Validators.max(30)]],
    });
  }

  onLogin(): void {
    if (this.form.invalid) {
      return;
    }

    this.authLoginService.handleAsync(this.form.value).subscribe({
      next: () => {
        const token = this.authService.getLoginToken();
        if (!token || !token.myAuthInfo) {
          console.error('No token after login');
          return;
        }

        const userId = this.authService.getCurrentUserId();
        if (userId) {
          this.chatService.startConnection();
        }

        const redirectUrl = localStorage.getItem('redirectAfterLogin');

        if (redirectUrl && token.myAuthInfo.isCustomer) {
          localStorage.removeItem('redirectAfterLogin');
          this.router.navigate([redirectUrl]);
        } else if (token.myAuthInfo.isAdmin) {
          this.router.navigate(['/admin/dashboard']);
        } else if (token.myAuthInfo.isPharmacist) {
          this.router.navigate(['/pharmacist/dashboard']);
        } else if (token.myAuthInfo.isCustomer) {
          this.router.navigate(['/public']);
        }
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.form.setErrors({ loginFailed: true });
        this.loginError = true;
      }
    });
  }
}
