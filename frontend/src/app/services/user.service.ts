import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';

export interface MyAppUserDTO {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  email: string;

}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly apiUrl = 'https://localhost:7057/api'; // prilagodi URL po potrebi

  constructor(private http: HttpClient) { }


  getPharmacists(): Observable<MyAppUserDTO[]> {
    return this.http.get<MyAppUserDTO[]>('https://localhost:7057/api/users/pharmacist');
  }



}
