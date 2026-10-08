import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../../my-config';

export interface RegisterRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface RegisterResponse {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthRegisterEndpointService {
  private apiUrl = `${MyConfig.api_address}/auth/register`;

  constructor(private httpClient: HttpClient) {}

  registerUser(data: RegisterRequest): Observable<RegisterResponse> {
    return this.httpClient.post<RegisterResponse>(this.apiUrl, data);
  }
}
