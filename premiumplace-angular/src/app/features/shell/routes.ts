import { Routes } from '@angular/router';
import { ShellLayoutComponent } from './shell-layout/shell-layout.component';

export const SHELL_ROUTES: Routes = [
    {
        path: '',
        component: ShellLayoutComponent,
        children: [
            {
                path: 'places',
                loadChildren: () =>
                    import('../places/places-page/routes').then(m => m.PLACES_ROUTES),
            },
            // още protected features: profile, admin...
        ],
    },
];
