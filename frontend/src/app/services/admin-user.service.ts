import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MyAppUser {
  id: number;
  username: string;
  email?: string;
  firstName: string;
  lastName: string;
  isAdmin: boolean;
  isPharmacist: boolean;
  isCustomer: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AdminUserService {
  private apiUrl  = 'https://localhost:7057/api/users';
  private baseUrl = 'https://localhost:7057';

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<MyAppUser[]> {
    return this.http.get<MyAppUser[]>(this.apiUrl);
  }

  // ✅ Nova metoda - backend filter + paging
  getUsersPaged(
    username?: string,
    firstName?: string,
    lastName?: string,
    role?: string,
    page: number = 1,
    pageSize: number = 10
  ): Observable<{ items: MyAppUser[], totalCount: number, page: number, pageSize: number, totalPages: number }> {
    let params: any = { page, pageSize };

    if (username)  params.username  = username;
    if (firstName) params.firstName = firstName;
    if (lastName)  params.lastName  = lastName;
    if (role)      params.role      = role;

    return this.http.get<any>(`${this.apiUrl}/paged`, { params });
  }

  updateUser(user: MyAppUser): Observable<void> {
    return this.http.put<void>(
      `https://localhost:7057/api/users/${user.id}`,
      user
    );
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(
      `https://localhost:7057/api/users/${id}`
    );
  }

  createUser(dto: {
    username: string;
    firstName: string;
    lastName: string;
    password: string;
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/register`, dto);
  }
}
