import { Component, OnInit } from '@angular/core';
import { PharmacistProfileService, PharmacistProfile, ChangePasswordDTO } from '../../../services/pharmacist-profile.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-pharmacist-dashboard',
  standalone: false,
  templateUrl: './pharmacist-dashboard.component.html',
  styleUrl: './pharmacist-dashboard.component.css'
})
export class PharmacistDashboardComponent implements OnInit {
  profile: PharmacistProfile = {} as PharmacistProfile;
  selectedFile: File | null = null;
  imagePreviewUrl: string | null = null;
  showPasswordModal = false;

  passwordData = {
    oldPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  constructor(
    private pharmacistProfileService: PharmacistProfileService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  get displayedProfileImageUrl(): string | undefined {
    return this.imagePreviewUrl ?? this.profile.profileImageUrl;
  }

  private loadProfile(): void {
    this.pharmacistProfileService.getProfile().subscribe({
      next: (data: PharmacistProfile) => {
        this.profile = data;
        this.imagePreviewUrl = null;
        this.selectedFile = null;
      },
      error: (error) => {
        console.error('Error fetching pharmacist profile:', error);

        if (error.status === 401) {
          this.router.navigate(['/auth/login']);
        }
      }
    });
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreviewUrl = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  updateProfile() {
    const formData = new FormData();
    formData.append('email', this.profile.email);

    if (this.selectedFile) {
      formData.append('profileImage', this.selectedFile, this.selectedFile.name);
    }

    this.pharmacistProfileService.updateProfile(formData).subscribe({
      next: (response) => {
        alert('Profile successfully updated!');
        this.profile.profileImageUrl = response.profileImageUrl ?? this.profile.profileImageUrl;
        this.imagePreviewUrl = null;
        this.selectedFile = null;
      },
      error: (error) => {
        console.error('Error:', error);
        alert(`Error : ${error.message}`);
      }
    });
  }

  changePassword() {
    if (!this.passwordData.oldPassword) {
      alert('Please enter your old password.');
      return;
    }

    if (!this.passwordData.newPassword) {
      alert('Please enter your new password.');
      return;
    }

    if (this.passwordData.newPassword.length < 4) {
      alert('The new password must have at least 4 characters.');
      return;
    }

    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      alert('The new passwords do not match.');
      return;
    }

    const dto: ChangePasswordDTO = {
      oldPassword: this.passwordData.oldPassword,
      newPassword: this.passwordData.newPassword
    };

    this.pharmacistProfileService.changePassword(dto).subscribe({
      next: () => {
        alert('Password successfully changed!');

        this.showPasswordModal = false;
        this.passwordData = {
          oldPassword: '',
          newPassword: '',
          confirmPassword: ''
        };
      },
      error: (err) => {
        console.error('Error changing password:', err);
      }
    });
  }
}
