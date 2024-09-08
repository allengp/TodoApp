import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TodoService } from '../../services/todo.service';
import { AuthService } from '../../services/auth.service';
import { Todo } from '../../models/todo';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent implements OnInit {
  todos: Todo[] = [];
  newTodo: Todo = { id: 0, title: '', isComplete: false };
  editMode: boolean = false;

  constructor(
    private todoService: TodoService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadTodos();
  }

  // Load todos from the service
  loadTodos(): void {
    this.todoService.getTodos().subscribe(todos => this.todos = todos);
  }

  // Add a new todo
  addTodo(): void {
    if (!this.newTodo.title.trim()) return;
    this.todoService.addTodo(this.newTodo).subscribe(todo => {
      this.todos.push(todo);
      this.newTodo = { id: 0, title: '', isComplete: false };
    });
  }

  // Edit a todo
  editTodo(todo: Todo): void {
    this.newTodo = { ...todo };
    this.editMode = true;
  }

  // Update the todo
  updateTodo(): void {
    if (this.newTodo.id) {
      this.todoService.updateTodo(this.newTodo.id, this.newTodo).subscribe(() => {
        const index = this.todos.findIndex(t => t.id === this.newTodo.id);
        if (index > -1) {
          this.todos[index] = { ...this.newTodo };
        }
        this.resetForm();
      });
    }
  }

  // Delete a todo
  deleteTodo(id: number): void {
    this.todoService.deleteTodo(id).subscribe(() => {
      this.todos = this.todos.filter(t => t.id !== id);
    });
  }

  // Reset the form
  resetForm(): void {
    this.newTodo = { id: 0, title: '', isComplete: false };
    this.editMode = false;
  }

  // Handle logout
  logout(): void {
    this.authService.logout();  // Call the logout method from AuthService
    this.router.navigate(['/login']);  // Navigate the user to the login page
  }
}
