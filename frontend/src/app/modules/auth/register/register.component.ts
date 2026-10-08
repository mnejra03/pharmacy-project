import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthRegisterEndpointService } from '../../../endpoints/auth-endpoints/auth-register-endpoint.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';


export interface RegisterResponse {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  imports: [
    ReactiveFormsModule,
    CommonModule,
  ],
  styleUrls: ['./register.component.css']

})
export class RegisterComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authRegisterService: AuthRegisterEndpointService,
    private router: Router
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
    });
  }
  onSubmit(): void {
    if (this.form.invalid) return;

    this.authRegisterService.registerUser(this.form.value).subscribe({
      next: (response) => {
        localStorage.setItem('userId', response.id.toString());
        alert('Uspješna registracija!');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        console.error('Greška prilikom registracije:', err);
        if (err && err.error) {
          console.error('Detalji greške:', err.error);
        }
        this.form.setErrors({ registrationFailed: true });
      }
    });
  }

}

