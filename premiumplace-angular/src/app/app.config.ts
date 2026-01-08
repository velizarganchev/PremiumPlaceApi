import { APP_INITIALIZER, ApplicationConfig, inject } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/http/auth.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './core/auth/auth.service';
import { refreshInterceptor } from './core/http/refresh.interceptor';

function initAuth() {
  return async () => {
    const auth = inject(AuthService);
    // Do not block app if unauthenticated
    try {
      return await firstValueFrom(auth.loadMe());
    } catch {
      return null;
    }
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor, refreshInterceptor, errorInterceptor])
    ),
    { provide: APP_INITIALIZER, multi: true, useFactory: initAuth }
  ]
};
