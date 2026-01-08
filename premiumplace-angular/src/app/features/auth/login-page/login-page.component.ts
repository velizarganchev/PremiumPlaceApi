import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    usernameOrEmail: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  onSubmit() {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    const dto = this.form.getRawValue();

    this.auth.login(dto).subscribe({
      next: () => {
        this.auth.loadMe().subscribe({
          next: () => this.router.navigateByUrl('/'),
          error: () => this.router.navigateByUrl('/'),
        });
      },
      error: (e: any) => {
        this.error.set(e?.message ?? 'Login failed');
        this.loading.set(false);
      }
    });
  }
}
