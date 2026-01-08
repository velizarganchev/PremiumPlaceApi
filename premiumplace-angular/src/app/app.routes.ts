import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    // default landing
    { path: '', pathMatch: 'full', redirectTo: 'places' },

    // public auth routes
    {
        path: 'auth',
        loadChildren: () =>
            import('./features/auth/routes').then(m => m.AUTH_ROUTES),
    },

    // protected app area (shell)
    {
        path: '',
        canActivate: [authGuard],
        loadChildren: () =>
            import('./features/shell/routes').then(m => m.SHELL_ROUTES),
    },

    // fallback
    { path: '**', redirectTo: 'places' },
];
