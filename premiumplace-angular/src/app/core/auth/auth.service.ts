import { inject, Injectable, signal, computed } from '@angular/core';
import { catchError, map, of, switchMap, tap } from 'rxjs';
import { AuthApi } from './auth.api';
import type { LoginRequest, RegisterRequest, User } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private api = inject(AuthApi);

    private _user = signal<User | null>(null);
    user = this._user.asReadonly();

    isLoggedIn = computed(() => this._user() !== null);

    // Load session from cookies
    loadMe() {
        return this.api.me().pipe(
            tap((u) => this._user.set(u)),
            catchError(() => {
                this._user.set(null);
                return of(null);
            })
        );
    }

    login(body: LoginRequest) {
        return this.api.login(body).pipe(
            switchMap(() => this.loadMe()),
            map(() => void 0)
        );
    }

    register(body: RegisterRequest) {
        return this.api.register(body);
    }

    logout() {
        return this.api.logout().pipe(
            tap(() => this._user.set(null))
        );
    }
}
