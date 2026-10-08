import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';

export interface Recipe {
  id: number;
  dateOfIssue: Date;
  doctorFirstname: string;
  doctorLastname: string;
  myAppUserId: number;
}

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  constructor(private http: HttpClient) { }
  private baseUrl = 'https://localhost:7057/api/recipes';

  updateStatus(recipeId: number, newStatus: string) {
    return this.http.put(`https://localhost:7057/api/recipes/${recipeId}/status`, { status: newStatus });
  }

  getMyRecipes() {
    return this.http.get<any[]>(`${this.baseUrl}/my`);
  }

  getAllRecipes(): Observable<any[]> {
    return this.http.get<any[]>('https://localhost:7057/api/recipes');
  }

  addRecipe(formData: FormData): Observable<any> {
    return this.http.post<any>('https://localhost:7057/api/recipes', formData);
  }

}
