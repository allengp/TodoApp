import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service'; // Assume AuthService is already created

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'todoapp.client';

  constructor(private authService: AuthService, private router: Router) { }

  // Check if user is logged in
  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  // Logout function
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
