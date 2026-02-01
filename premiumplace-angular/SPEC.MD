# PremiumPlace Angular – Technical Specification

Version: 1.1
Last updated: February 2026
Project: premiumplace-angular

---

## 1. Purpose of This Document

This document defines the **technical architecture, design decisions, and implementation standards** for the **PremiumPlace Angular SPA**.

It serves as:

- a long-term **technical foundation**
- onboarding documentation for developers
- a reference for future feature development (bookings, calendar, payments)

This specification is **frontend-only** and focuses exclusively on the Angular application.

---

## 2. High-Level Architecture

### 2.1 Architectural Principles

- **API-first**: Angular consumes a stateless ASP.NET Core REST API
- **Read-only aggregation**: all calculated values (review summary, counts, averages) are produced by the API
- **Signal-driven state**: Angular Signals are the primary state mechanism
- **Standalone components**: no NgModules
- **Explicit data mapping**: DTO → UI models via mappers
- **Public browsing, protected actions**: browsing is public; booking requires authentication
- **Lazy-loaded routes**: all feature modules are loaded on demand

---

## 3. Technology Stack

- Angular 17+ (standalone APIs)
- Angular Router (lazy loading)
- Angular Signals (`signal`, `computed`, `effect`)
- Angular Material (custom black/white theme)
- RxJS (HTTP + orchestration only)
- HttpOnly-cookie-based JWT authentication
- SCSS (global theme + component scoping)

---

## 4. Application Structure

```
src/app/
│
├── core/
│   ├── auth/
│   │   ├── auth.api.ts
│   │   ├── auth.models.ts
│   │   └── auth.service.ts
│   │
│   ├── http/
│   │   ├── api.constants.ts
│   │   ├── auth.interceptor.ts
│   │   ├── error.interceptor.ts
│   │   ├── error.types.ts
│   │   └── refresh.interceptor.ts
│   │
│   ├── guards/
│   │   └── auth.guard.ts
│   │
│   └── places/
│       ├── places.api.ts
│       ├── places.mapper.ts
│       ├── places.models.ts
│       └── places.service.ts
│
├── features/
│   ├── auth/
│   │   ├── login-page/
│   │   │   ├── login-page.component.ts
│   │   │   ├── login-page.component.html
│   │   │   └── login-page.component.scss
│   │   └── routes.ts
│   │
│   ├── home/
│   │   ├── hero/
│   │   │   ├── hero.component.ts
│   │   │   ├── hero.component.html
│   │   │   └── hero.component.scss
│   │   ├── home.component.ts
│   │   ├── home.component.html
│   │   └── home.component.scss
│   │
│   ├── places/
│   │   ├── place-details/
│   │   │   ├── place-details.component.ts
│   │   │   ├── place-details.component.html
│   │   │   └── place-details.component.scss
│   │   ├── places-page/
│   │   │   ├── places-page.component.ts
│   │   │   ├── places-page.component.html
│   │   │   └── places-page.component.scss
│   │   └── routes.ts
│   │
│   └── shell/
│       ├── shell-layout/
│       │   ├── shell-layout.component.ts
│       │   ├── shell-layout.component.html
│       │   └── shell-layout.component.scss
│       └── routes.ts
│
├── shared/
│   └── ui/
│       ├── cards-grid/
│       │   ├── cards-grid.component.ts
│       │   ├── cards-grid.component.html
│       │   ├── cards-grid.component.scss
│       │   └── cards-grid.model.ts
│       ├── footer/
│       │   ├── footer.component.ts
│       │   ├── footer.component.html
│       │   └── footer.component.scss
│       └── navbar/
│           ├── navbar.component.ts
│           ├── navbar.component.html
│           └── navbar.component.scss
│
├── app.component.ts
├── app.config.ts
└── app.routes.ts
```

---

## 5. Routing Strategy

### 5.1 Root Routes

All routes use **lazy loading** for optimal performance:

```ts
// app.routes.ts
export const routes: Routes = [
    {
        path: '',
        loadChildren: () =>
            import('./features/shell/routes').then(m => m.SHELL_ROUTES),
    },
    {
        path: 'auth',
        loadChildren: () =>
            import('./features/auth/routes').then(m => m.AUTH_ROUTES),
    },
    { path: '**', redirectTo: '' },
];
```

### 5.2 Shell Routes

The Shell Layout wraps all public pages and provides:

- top navigation (navbar)
- footer
- consistent spacing and theme

```ts
// features/shell/routes.ts
export const SHELL_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./shell-layout/shell-layout.component')
                .then(m => m.ShellLayoutComponent),
        children: [
            {
                path: '',
                loadComponent: () =>
                    import('../home/home.component').then(m => m.HomeComponent),
            },
            {
                path: 'places',
                loadChildren: () =>
                    import('../places/routes').then(m => m.PLACES_ROUTES),
            },
        ],
    },
];
```

### 5.3 Places Routes

```ts
// features/places/routes.ts
export const PLACES_ROUTES: Routes = [
    {
        path: ':id',
        loadComponent: () =>
            import('./place-details/place-details.component')
                .then(m => m.PlaceDetailsComponent)
    },
];
```

### 5.4 Auth Routes

```ts
// features/auth/routes.ts
export const AUTH_ROUTES: Routes = [
    { path: 'login', component: LoginPageComponent },
];
```

### 5.5 Access Control

- Browsing places → public
- Booking (future) → protected via `authGuard`
- Login/register routes live outside Shell layout

---

## 6. State Management Strategy

### 6.1 Signals as Source of Truth

- Services own state (`signal`)
- Components consume read-only projections (`computed`, `asReadonly()`)
- No shared mutable state in components

Example:

```ts
// PlacesService
private _places = signal<PlacePreview[]>([]);
private _loadingList = signal(false);

places = this._places.asReadonly();
loadingList = this._loadingList.asReadonly();

cards = computed(() => this.places().map(mapPlaceToCard));
```

### 6.2 RxJS Usage Policy

RxJS is used **only** for:

- HTTP requests
- orchestration (login → loadMe → redirect)

Signals are used for:

- state
- derived values
- UI reactivity

---

## 7. Places Domain (Frontend)

### 7.1 Models

**PlaceFeatures**

```ts
export type PlaceFeatures = {
    internet: boolean;
    airConditioned: boolean;
    petsAllowed: boolean;
    parking: boolean;
    entertainment: boolean;
    kitchen: boolean;
    refrigerator: boolean;
    washer: boolean;
    dryer: boolean;
    selfCheckIn: boolean;
};
```

**PlacePreview** (list view)

```ts
export type PlacePreview = {
    id: number;
    name: string;
    details: string;
    city: string;
    rate: number;
    imageUrl: string;
    amenity: string[];
    features: PlaceFeatures;
    guestCapacity: number;
    beds: number;
};
```

**PlaceDto** (detail view)

```ts
export type PlaceDto = {
    id: number;
    name: string;
    details: string;
    guestCapacity: number;
    rate: number;
    beds: number;
    checkInHour: number;
    checkOutHour: number;
    squareFeet: number;
    imageUrl: string;
    city: string;
    features: PlaceFeatures;
    amenitys: string[];
    reviewSummary: {
        avg: number;
        count: number;
    };
    reviews: Review[];
};

type Review = {
    id: number;
    rating: number;
    comment: string;
    createdAt: string;
    userId: number;
    username: string;
};
```

**CardItem** (shared UI model)

```ts
export type CardItem = {
    id: string;
    title: string;
    subtitle: string;
    imageUrl: string;
    priceText?: string;
    meta?: string;
    href?: string;
};
```

### 7.2 PlacesService Responsibilities

- Load places list (`loadAll()`)
- Load place details by ID (`byId(id)`)
- Expose loading states
- Provide computed `cards` signal for UI
- Never calculate aggregates

The API is the single authority for:

- average rating
- review count
- review ordering

---

## 8. Place Details Page

### 8.1 Responsibilities

- Render place header (name, city)
- Render rating summary (stars + count)
- Render image gallery
- Render amenities and features
- Render reviews list
- Provide "Back to Home" navigation

### 8.2 Stars Calculation

Stars are computed client-side for display purposes only:

```ts
stars = computed(() => {
    const avg = this.place()!.reviewSummary.avg;
    const full = Math.floor(avg);
    const half = avg - full >= 0.5;
    const empty = 5 - full - (half ? 1 : 0);
    return { full, half, empty };
});
```

### 8.3 Review Rendering

Reviews are:

- sorted server-side
- immutable on the client
- displayed exactly as returned

The frontend does **not**:

- calculate averages
- filter reviews
- aggregate ratings

---

## 9. Authentication Model

### 9.1 Models

```ts
export type User = {
    id: number;
    username: string;
    email: string;
    roles?: string[];
};

export type LoginRequest = { usernameOrEmail: string; password: string };
export type RegisterRequest = { username: string; email: string; password: string };
```

### 9.2 Strategy

- JWT stored in HttpOnly cookies
- No localStorage/sessionStorage
- Auth state restored via `/auth/me` on app init

### 9.3 AuthService

```ts
private _user = signal<User | null>(null);
user = this._user.asReadonly();
isLoggedIn = computed(() => this._user() !== null);
```

### 9.4 Interceptors

| Interceptor | Responsibility |
|-------------|----------------|
| `auth` | attach cookies (`withCredentials`) |
| `refresh` | refresh token on 401 |
| `error` | normalize API errors |

---

## 10. Shared UI Components

### 10.1 Navbar

Located in `shared/ui/navbar/`. Provides top navigation across all Shell pages.

### 10.2 Footer

Located in `shared/ui/footer/`. Consistent footer across all Shell pages.

### 10.3 Cards Grid

Located in `shared/ui/cards-grid/`. Reusable grid component for displaying `CardItem[]`.

---

## 11. UI & Theming

### 11.1 Design Language

- Black / white / grey base
- Subtle blue accent
- Minimalist, content-first

### 11.2 Angular Material Usage

- Buttons, cards, icons, dividers
- Custom SCSS theme
- No inline styling

---

## 12. Error Handling

- Centralized via `errorInterceptor`
- API errors normalized into `ApiError` type
- Components react declaratively

---

## 13. Performance & UX

- Router scroll restoration enabled
- Lazy-loaded feature routes (implemented)
- No unnecessary re-fetching
- Computed signals for derived UI state

---

## 14. Future Extensions (Angular-side)

- [ ] Register page component
- [ ] Places list page (currently only detail view routed)
- [ ] Booking calendar component
- [ ] Availability visualization
- [ ] Auth-gated booking flow
- [ ] Review submission UI
- [ ] Payment UI integration

---

## 15. Non-Goals

This Angular app intentionally does **not**:

- implement business logic
- compute aggregates
- manage persistence
- duplicate backend validation

---

## 16. Summary

The PremiumPlace Angular SPA is designed as:

- a clean, reactive, API-driven client
- a long-term scalable frontend foundation
- a reference-grade Angular architecture

All business complexity lives in the API.
Angular focuses on **presentation**, **composition**, and **UX**.
