import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Login } from '../models/login';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth';  // Backend API URL

  constructor(private http: HttpClient) { }

  // Method to handle user login
  login(loginData: Login): Observable<boolean> {
    return new Observable<boolean>(observer => {
      this.http.post<{ token: string }>(`${this.apiUrl}/login`, loginData).subscribe(
        response => {
          // Store the token in localStorage
          localStorage.setItem('token', response.token);
          observer.next(true);  // Notify that login is successful
          observer.complete();
        },
        error => {
          console.error('Login error:', error);
          observer.next(false);  // Notify that login failed
          observer.complete();
        }
      );
    });
  }

  // Method to get the stored token
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Method to check if the user is logged in
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');  // Returns true if token exists
  }

  // Method to log out the user
  logout(): void {
    localStorage.removeItem('token');  // Remove the token from localStorage
  }
}

