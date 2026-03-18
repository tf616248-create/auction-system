import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatIconModule, MatButtonModule, CommonModule],
  template: `
    <div class="toolbar">
      <button mat-icon-button (click)="toggleTheme()">
        <mat-icon>{{ isDark ? 'light_mode' : 'dark_mode' }}</mat-icon>
      </button>
    </div>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .toolbar {
      display: flex;
      justify-content: flex-left;
      padding: 8px;
      background-color: var(--card-bg);
      transition: background-color 0.3s;
    }
  `]
})
export class AppComponent {
  isDark = false;

  toggleTheme(): void {
    this.isDark = !this.isDark;
    document.documentElement.setAttribute(
      'data-theme',
      this.isDark ? 'dark' : 'light'
    );
  }
}