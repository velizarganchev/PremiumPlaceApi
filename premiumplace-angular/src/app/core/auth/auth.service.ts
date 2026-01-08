import { inject, Injectable, signal, computed } from '@angular/core';
import { tap } from 'rxjs';
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
            tap({
                next: (u) => this._user.set(u),
                error: () => this._user.set(null),
            })
        );
    }

    login(body: LoginRequest) {
        // Cookies are set by API; then we fetch /me to hydrate state
        return this.api.login(body);
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
