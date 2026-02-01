# PremiumPlace – Business Specification

Version: 1.1
Last updated: February 2026

---

## 1. Introduction

### 1.1 Product Vision

**PremiumPlace** is a short-term rental platform for accommodations, inspired by Airbnb. The platform connects property owners (hosts) with guests looking for unique places to stay.

### 1.2 Business Goals

| Goal | Description |
|------|-------------|
| **Connection** | Facilitate communication between hosts and guests |
| **Accessibility** | Provide an intuitive interface for browsing and bookings |
| **Trust** | Build a reviews and ratings system |
| **Scalability** | A platform ready to expand into new markets |

### 1.3 Target Audience

**Guests (Users):**
- Tourists seeking an alternative to hotels
- Business travelers needing short stays
- Families looking for spacious and comfortable places
- Digital nomads

**Owners (Hosts):**
- Individuals with an additional property
- Property managers
- Small hotels and guest houses

---

## 2. Core Functionalities

### 2.1 Currently Implemented Features

#### 2.1.1 Place Management (Places)

| Feature | Description | Status |
|---------|-------------|--------|
| View all places | List of all available accommodation places | ✅ Done |
| Place details | Full information for a specific place | ✅ Done |
| Create place | Add a new place to the system | ✅ Done |
| Edit place | Full or partial update (PUT/PATCH) | ✅ Done |
| Delete place | Remove a place from the system | ✅ Done |

**Place information includes:**
- Name and description
- Guest capacity (1–30)
- Number of beds (0–20)
- Price per night (rate)
- Size (10–10,000 sq ft)
- Check-in / check-out hours
- Photo (imageUrl)
- City / location
- Features and amenities

#### 2.1.2 Place Features (Features)

Each place can have the following features:

| Category | Features |
|----------|----------|
| **Connectivity** | Internet |
| **Climate** | Air conditioning |
| **Pets** | Allowed / Not allowed |
| **Parking** | Available / Not available |
| **Entertainment** | Entertainment system |
| **Kitchen** | Kitchen, Refrigerator |
| **Laundry** | Washer, Dryer |
| **Check-in** | Self check-in |

#### 2.1.3 Amenities (Amenities)

A flexible system for adding additional amenities to each place. Amenities are independent records that can be associated with multiple places (many-to-many relationship).

**Seeded amenities:**
- Free Wi-Fi
- Swimming Pool
- Gym Access
- 24/7 Room Service
- Airport Shuttle

#### 2.1.4 Authentication and Users

| Feature | Description | Status |
|---------|-------------|--------|
| Registration | Create a new account with auto-login | ✅ Done |
| Login | Authenticate using email/username and password | ✅ Done |
| Logout | End the session | ✅ Done |
| Profile | View current user (GET /auth/me) | ✅ Done |
| Delete account | Permanent deletion with password confirmation | ✅ Done |
| Token refresh | Automatic token rotation | ✅ Done |

**Security:**
- JWT tokens for authentication
- Refresh tokens with automatic rotation
- HttpOnly cookies for XSS protection
- Password hashing
- IP address tracking for tokens
- Token revocation support

#### 2.1.5 Reviews System (Reviews)

| Feature | Description | Status |
|---------|-------------|--------|
| View reviews | Reviews displayed on place details | ✅ Done |
| Review summary | Average rating and count per place | ✅ Done |
| Rating | Rating from 1 to 5 stars | ✅ Done |
| Comment | Text review up to 1000 characters | ✅ Done |
| Uniqueness | One review per user per place (DB constraint) | ✅ Done |
| Create review | Add a new review | ⏳ Pending |
| Update review | Edit existing review | ⏳ Pending |
| Delete review | Remove a review | ⏳ Pending |

> **Note:** Review model and database constraints are implemented. Reviews are displayed in place details with summary (avg/count). However, API endpoints for creating, updating, and deleting reviews are not yet implemented.

---

### 2.2 Partially Implemented Features

#### 2.2.1 Authorization & Roles

| Feature | Description | Status |
|---------|-------------|--------|
| User roles | User and Admin roles defined | ✅ Done |
| Role-based access | Endpoint authorization | ⏳ Pending |

> **Note:** User/Admin roles are defined in the system, but `[Authorize]` attributes are not applied to Place CRUD endpoints. Currently, any authenticated user can create/edit/delete any place.

---

### 2.3 Planned Features

#### 2.3.1 Booking System (Bookings)

**Expected process:**

```
┌─────────────────┐
│ 1. Select place │
└───────┬─────────┘
        ▼
┌─────────────────┐
│ 2. Select dates │
└───────┬─────────┘
        ▼
┌─────────────────┐
│ 3. Availability │
│     check       │
└───────┬─────────┘
        ▼
┌─────────────────┐
│ 4. Create       │
│    booking      │
└───────┬─────────┘
        ▼
┌─────────────────┐
│ 5. Payment      │
└───────┬─────────┘
        ▼
┌─────────────────┐
│ 6. Confirmation │
└─────────────────┘
```

**Booking statuses:**
- `Pending` – awaiting confirmation
- `Confirmed` – confirmed
- `Cancelled` – cancelled
- `Completed` – completed

#### 2.3.2 Availability Calendar

- Visualization of booked/available dates
- Host-blocked periods
- Automatic updates after bookings

#### 2.3.3 Payment System

**Planned integrations:**
- Stripe
- PayPal

**Business model:**
- Commission per booking
- Secure payment processing
- Buyer protection

#### 2.3.4 Admin Panel

- User management
- View all bookings
- Statistics and reports
- Review moderation
- Content management

#### 2.3.5 Email Notifications

- Booking confirmation emails
- Reminder emails
- Review request emails

---

## 3. User Roles and Permissions

### 3.1 Role Hierarchy

```
┌─────────────────────────────────────────┐
│                 Admin                    │
│       (Full access to the system)        │
├─────────────────────────────────────────┤
│                  User                    │
│          (Registered user)               │
├─────────────────────────────────────────┤
│                 Guest                    │
│          (Anonymous visitor)             │
└─────────────────────────────────────────┘
```

### 3.2 Permissions Matrix

| Action | Guest | User | Admin |
|--------|:-----:|:----:|:-----:|
| View places | ✅ | ✅ | ✅ |
| View details | ✅ | ✅ | ✅ |
| Register | ✅ | - | - |
| Login | - | ✅ | ✅ |
| Create place | - | ⏳* | ✅ |
| Edit place | - | ⏳** | ✅ |
| Delete place | - | ⏳** | ✅ |
| Write review | - | ⏳ | ⏳ |
| Booking (future) | - | ⏳ | ⏳ |
| Admin panel | - | - | ⏳ |

\* Authorization pending implementation
\** Only own places (pending implementation)

---

## 4. User Stories

### 4.1 Guest

> As an anonymous visitor,
> I want to browse available places,
> so I can decide whether to register.

> As an anonymous visitor,
> I want to see full place details,
> so I can make an informed decision.

### 4.2 User

> As a registered user,
> I want to book a place for specific dates,
> so I can plan my stay.

> As a registered user,
> I want to leave a review after my stay,
> so I can help other guests.

> As a place owner,
> I want to publish my property,
> so I can attract guests.

> As a place owner,
> I want to manage my bookings,
> so I can organize rentals.

### 4.3 Admin

> As an administrator,
> I want to view all bookings,
> so I can monitor platform activity.

> As an administrator,
> I want to moderate content,
> so I can maintain platform quality.

---

## 5. Business Processes

### 5.1 Registration Process

```
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  Input data  │───▶│  Validation  │───▶│   Create     │
│              │    │              │    │   account    │
└──────────────┘    └──────────────┘    └──────────────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │  Auto-login  │
                                        └──────────────┘
```

**Validations:**
- Unique email
- Unique username
- Strong password

### 5.2 Publishing a Place

```
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ Enter details│───▶│  Add photos  │───▶│   Choose     │
│              │    │              │    │  amenities   │
└──────────────┘    └──────────────┘    └──────────────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │   Publish    │
                                        └──────────────┘
```

### 5.3 Booking Process (Planned)

```
Search ──▶ Select ──▶ Dates ──▶ Payment ──▶ Confirmation
                                               │
                                               ▼
                                        ┌──────────┐
                                        │ Notices  │
                                        │ to both  │
                                        │ parties  │
                                        └──────────┘
```

---

## 6. Geographic Coverage

### 6.1 Currently Supported Cities

| City | Country |
|------|---------|
| Berlin | Germany |
| Munich | Germany |
| Hamburg | Germany |
| Frankfurt | Germany |
| Cologne | Germany |

### 6.2 Planned Expansion

- Additional German cities
- Expansion to other European markets
- Multilingual UI

---

## 7. Technical Requirements (Business Perspective)

### 7.1 Performance

| Metric | Target |
|--------|--------|
| Page load time | < 3 seconds |
| API response time | < 500ms |
| Availability (Uptime) | 99.5% |

### 7.2 Security

- Encryption of sensitive data
- Protection against CSRF/XSS attacks
- Secure password storage
- PCI DSS compliance (payments)

### 7.3 Scalability

- Horizontal scaling of the API
- CDN for static assets
- Caching frequently requested data

---

## 8. Success Metrics (KPIs)

### 8.1 Business Metrics

| Metric | Description |
|--------|-------------|
| **Number of registrations** | New users per month |
| **Number of active places** | Total published places |
| **Occupancy rate** | % of booked nights |
| **Average rating** | Average platform rating |
| **Retention rate** | % returning users |

### 8.2 Technical Metrics

| Metric | Description |
|--------|-------------|
| **Conversion rate** | % visits leading to bookings |
| **Bounce rate** | % leaving without action |
| **Error rate** | % failed requests |
| **Response time** | Average response time |

---

## 9. Competitive Advantages

| Advantage | Description |
|----------|-------------|
| **Modern architecture** | API-first approach for flexibility |
| **Rapid development** | Ready for new features |
| **Security** | Modern protection practices |
| **UX/UI** | Intuitive interface with Material Design |

---

## 10. Risks and Mitigations

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Low user base | Medium | High | Marketing campaigns |
| Technical issues | Low | Medium | Monitoring and alerts |
| Competition | High | Medium | Unique features |
| Regulatory changes | Medium | Medium | Legal consulting |

---

## 11. Roadmap

### Phase 1 – MVP (Current)

- [x] Registration and authentication
- [x] Token refresh with rotation
- [x] CRUD operations for places
- [x] Place features and amenities
- [x] Reviews display with summary
- [x] Basic Angular UI
- [ ] Review CRUD endpoints
- [ ] Endpoint authorization (role-based)

### Phase 2 – Bookings (Q2 2026)

- [ ] Booking system
- [ ] Availability calendar
- [ ] Email notifications

### Phase 3 – Payments (Q3 2026)

- [ ] Stripe integration
- [ ] PayPal support
- [ ] Invoicing

### Phase 4 – Expansion (Q4 2026)

- [ ] Admin panel
- [ ] Multilingual support
- [ ] Mobile applications

---

## 12. Technical Implementation Status

### 12.1 Backend (ASP.NET Core 10.0)

| Component | Status |
|-----------|--------|
| AuthController | ✅ Complete |
| PlacesController | ✅ Complete |
| ReviewsController | ⏳ Pending |
| BookingsController | ⏳ Pending |
| AdminController | ⏳ Pending |

### 12.2 Frontend (Angular 17+)

| Component | Status |
|-----------|--------|
| Shell layout | ✅ Complete |
| Home page | ✅ Complete |
| Place details | ✅ Complete |
| Login page | ✅ Complete |
| Register page | ⏳ Pending |
| Places list page | ⏳ Pending |
| Booking flow | ⏳ Pending |

### 12.3 Database

| Entity | Status |
|--------|--------|
| User | ✅ Complete |
| Place | ✅ Complete |
| PlaceFeatures | ✅ Complete |
| Amenity | ✅ Complete |
| City | ✅ Complete |
| Review | ✅ Complete |
| RefreshToken | ✅ Complete |
| Booking | ⏳ Pending |

---

## 13. Conclusion

**PremiumPlace** is built with a focus on:

1. **Quality** – Reliable and secure platform
2. **Flexibility** – Easy addition of new features
3. **User experience** – Intuitive and modern interface
4. **Scalability** – Prepared for growth

The platform represents a solid foundation for building a business in the short-term rental space.

---

*Version: 1.1*
*Last updated: February 2026*
