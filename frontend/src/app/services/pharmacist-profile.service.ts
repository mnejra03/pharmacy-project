import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PharmacistProfile {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  employmentDate: string;
  profileImageUrl?: string;
}

export interface ChangePasswordDTO {
  oldPassword: string;
  newPassword: string;
}

@Injectable({
  providedIn: 'root'
})
export class PharmacistProfileService {
  private readonly API_URL = 'https://localhost:7057/api/PharmacistDashboardEndpoint';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<PharmacistProfile> {
    return this.http.get<PharmacistProfile>(`${this.API_URL}/pharmacist-profile`);
  }

  updateProfile(formData: FormData): Observable<any> {
    return this.http.put(`${this.API_URL}/update-profile`, formData);
  }

  changePassword(dto: ChangePasswordDTO): Observable<any> {
    return this.http.post(`${this.API_URL}/change-password`, dto);
  }
}
