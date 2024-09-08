import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { Login } from '../../models/login';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData: Login = { username: '', password: '' };
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  // Handle login
  login(): void {
    this.authService.login(this.loginData).subscribe(
      success => {
        if (success) {
          this.router.navigate(['/']);  // Redirect to home or todo list
        } else {
          this.errorMessage = 'Invalid username or password.';
        }
      },
      error => {
        this.errorMessage = 'An error occurred during login. Please try again.';
      }
    );
  }
}
