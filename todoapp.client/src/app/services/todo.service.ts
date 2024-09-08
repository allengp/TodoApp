import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo } from '../models/todo';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = '/api/todo';  // Backend API URL

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Method to get all todos
  getTodos(): Observable<Todo[]> {
    const token = this.authService.getToken();  // Get the token from AuthService
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);  // Add the token to the headers

    return this.http.get<Todo[]>(this.apiUrl, { headers });  // Send the request with the token
  }

  // Method to add a new todo
  addTodo(todo: Todo): Observable<Todo> {
    const token = this.authService.getToken();  // Get the token from AuthService
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<Todo>(this.apiUrl, todo, { headers });
  }

  // Method to update an existing todo
  updateTodo(id: number, todo: Todo): Observable<void> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put<void>(`${this.apiUrl}/${id}`, todo, { headers });
  }

  // Method to delete a todo
  deleteTodo(id: number): Observable<void> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }
}
