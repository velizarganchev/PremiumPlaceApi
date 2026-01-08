import { HttpInterceptorFn } from '@angular/common/http';
import { API_PREFIX } from './api.constants';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    // Attach cookies only for API calls (HttpOnly cookies)
    if (req.url.startsWith(API_PREFIX)) {
        return next(req.clone({ withCredentials: true }));
    }
    return next(req);
};
