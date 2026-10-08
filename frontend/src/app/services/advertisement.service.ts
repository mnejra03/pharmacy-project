import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Advertisement {
  id: number;
  title: string;
  imageURL: string;
}

@Injectable({
  providedIn: 'root',
})
export class AdvertisementService {
  private apiUrl = 'https://localhost:7057/api/GetAdvertisementEndpoint';

  constructor(private http: HttpClient) {}

  getAdvertisements(): Observable<Advertisement[]> {
    return this.http.get<Advertisement[]>(this.apiUrl);
  }
}

