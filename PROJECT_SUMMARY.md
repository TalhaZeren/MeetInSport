# MeetInSport — Comprehensive Project Summary Prompt

> **Purpose:** This document is a full-detail context prompt for continuing development of the MeetInSport backend in a new LLM conversation. It covers architecture, all entities, all DTOs, all services, all repositories, all controllers, configurations, auth flow, known bugs fixed, and what remains to be built.

---

## 1. Project Overview

**MeetInSport** is a sport-coaching platform API that connects **coaches** and **students**. Coaches publish lesson packages (e.g. "Tennis Beginner – 60 min, 150 TRY"), and students can browse and reserve them. The backend is a **.NET 8 RESTful Web API** following **Clean Architecture**, deployed with **Docker + PostgreSQL 16**.

### Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 (C#) |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 (Npgsql) |
| Database | PostgreSQL 16 |
| Authentication | JWT Bearer (HS256) |
| Password Hashing | BCrypt.Net |
| Object Mapping | AutoMapper 12 |
| Containerization | Docker + Docker Compose |
| API Documentation | Swagger (Swashbuckle) |

---

## 2. Solution Structure (Clean Architecture — 4 Projects)

```
MeetInSport.sln
├── src/
│   ├── MeetInSport.Domain                      → Entities, Enums, BaseEntity (no dependencies)
│   ├── MeetInSport.Application                 → Interfaces, Services, DTOs, Mappings, Exceptions
│   ├── MeetInSport.Infrastructure.Persistence  → EF Core, Repositories, Migrations, Seeder, DI
│   └── MeetInSport.WebApi                      → Controllers, Middleware, Program.cs
```

### Dependency direction (Clean Architecture rule):
```
WebApi → Application → Domain
Infrastructure.Persistence → Application → Domain
```
Infrastructure.Persistence and WebApi both depend on Application. Neither Application nor Domain knows about the outer layers.

---

## 3. Domain Layer — `MeetInSport.Domain`

### 3.1 BaseEntity (abstract, inherited by all entities)

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

### 3.2 Entities

#### `User : BaseEntity`
| Property | Type | Notes |
|---|---|---|
| Name | string | |
| Email | string | unique |
| PasswordHash | string | BCrypt hash |
| RoleId | int | FK → Role.Id |
| PhoneNumber | string? | |
| AvatarUrl | string? | |
| KvkkAcceptedAt | DateTime | GDPR/KVKK consent |
| LastLoginAt | DateTime? | |
| IsActive | bool | default: true |
| IsEmailVerified | bool | default: false |
| Role | virtual Role | nav |
| CoachProfile | virtual Coach? | 1-to-1, nullable (not all users are coaches) |
| Reservations | virtual ICollection\<Reservation\> | 1-to-N |
| AuditLogs | virtual ICollection\<AuditLog\> | 1-to-N |

#### `Coach : BaseEntity`
| Property | Type | Notes |
|---|---|---|
| UserId | Guid | FK → User.Id |
| Sport | string | e.g. "Tennis" |
| Bio | string? | |
| HourlyRate | decimal | |
| Experience | int | years |
| IsApproved | bool | default: false, admin must approve |
| AverageRating | decimal | |
| Location | string? | |
| Iban | string? | for payouts |
| User | virtual User | nav |
| Packages | virtual ICollection\<LessonPackage\> | 1-to-N |
| Reservations | virtual ICollection\<Reservation\> | 1-to-N |

#### `LessonPackage : BaseEntity`
| Property | Type | Notes |
|---|---|---|
| CoachId | Guid | FK → Coach.Id |
| PackageName | string | max 200 chars, unique per coach |
| PackageDescription | string | max 500 chars |
| DurationInMinutes | decimal | |
| PackagePrice | decimal | precision(18,2) |
| Requirements | **List\<string\>** | PostgreSQL `text[]` — **MUST be List\<string\>, NOT ICollection\<string\>** (EF materializer bug fix) |
| LocationType | LocationType (enum) | CoachLocation / StudentLocation / Online |
| IsActive | bool | |
| LessonModel | LessonModel (enum) | OneOnOne / Group |
| CoverImageUrl | string? | max 500 chars |
| Coach | virtual Coach | nav |
| Reservations | virtual ICollection\<Reservation\> | 1-to-N |

#### `Reservation : BaseEntity`
| Property | Type | Notes |
|---|---|---|
| StudentId | Guid | FK → User.Id |
| CoachId | Guid | FK → Coach.Id |
| PackageId | Guid | FK → LessonPackage.Id |
| ScheduledAt | DateTime | |
| LocationType | LocationType (enum) | |
| Status | ReservationStatus (enum) | Pending/Confirmed/Cancelled/Completed/Refunded |
| Notes | string? | |
| CancelledAt | DateTime | |
| CancelReason | string? | |
| Student | virtual User | nav |
| Coach | virtual Coach | nav |
| Package | virtual LessonPackage | nav |
| Payment | virtual Payment? | 1-to-1 optional |

#### `Payment : BaseEntity`
| Property | Type | Notes |
|---|---|---|
| ReservationId | Guid | FK → Reservation.Id |
| Amount | decimal | |
| Currency | string | default: "TRY" |
| Status | PaymentStatus (enum) | Pending/Completed/Failed/Refunded |
| TransactionId | string? | |
| ProcessedAt | DateTime? | |
| Reservation | virtual Reservation | nav |

#### `Role` (no BaseEntity)
| Property | Type |
|---|---|
| Id | int |
| RoleName | string |
| Description | string? |
| Users | virtual ICollection\<User\> |

**Seeded Roles:**
| Id | RoleName |
|---|---|
| 1 | Student |
| 2 | Coach |
| 3 | Admin |

#### `AuditLog` (no BaseEntity — uses long Id for high-frequency writes)
| Property | Type | Notes |
|---|---|---|
| Id | long | 64-bit to avoid overflow |
| UserId | Guid | FK → User.Id |
| Action | string | CREATE / UPDATE / DELETE |
| EntityType | string | which entity was affected |
| EntityId | Guid? | |
| OldValue | string? | JSON snapshot |
| NewValue | string? | JSON snapshot |
| IpAddress | string? | |
| Timestamp | DateTime | UTC |
| User | virtual User? | nav |

### 3.3 Enums

```csharp
// LocationType
CoachLocation = 1, StudentLocation = 2, Online = 3

// LessonModel
OneOnOne = 1, Group = 2

// ReservationStatus
Pending = 1, Confirmed = 2, Cancelled = 3, Completed = 4, Refunded = 5

// PaymentStatus
Pending = 1, Completed = 2, Failed = 3, Refunded = 4
```

---

## 4. Application Layer — `MeetInSport.Application`

### 4.1 Services (Interfaces + Implementations)

#### `IAuthService` / `AuthService`
- **`RegisterAsync(RegisterRequestDto)`** → `AuthResponseDto`
  - Checks email uniqueness
  - BCrypt hashes password
  - Creates `User` entity (RoleId from request)
  - If `RoleId == 2` (Coach), automatically creates a linked `Coach` profile with default placeholders
  - Saves to DB
- **`LoginAsync(LoginRequestDto)`** → `LoginResponseDto`
  - Validates email + BCrypt verifies password
  - Generates JWT token with claims: `NameIdentifier (UserId)`, `Email`, `Name`, `Role`
  - JWT expires in 7 days, signed with HS256

#### `ICoachService` / `CoachService`
- **`GetAllCoachesAsync()`** → `IEnumerable<CoachResponseDto>`
  - Uses `GetAllCoachesWithDetailsAsync()` which does `.Include(c => c.User)` JOIN
  - Mapped via AutoMapper (FullName from `User.Name`)
- **`GetCoachByIdAsync(Guid)`** → `CoachResponseDto?`
  - Uses generic `GetByIdAsync`
- **`GetCoachesBySportAsync(string)`** → `IEnumerable<CoachResponseDto>`
  - Case-insensitive sport filter, only approved coaches

#### `ILessonPackageService` / `LessonPackageService`
- **`CreatePackageAsync(CreateLessonPackageDto, Guid currentUserId)`** → `LessonPackageResponseDto`
  - Looks up coach by `currentUserId` (userId from JWT, not frontend)
  - Builds `LessonPackage` entity, saves, returns mapped DTO
- **`GetPackagesByCoachIdAsync(Guid coachId)`** → `IEnumerable<LessonPackageResponseDto>`
  - Queries packages by `CoachId`, maps via AutoMapper

### 4.2 DTOs

**Auth:**
```csharp
// RegisterRequestDto
{ Name, Email, Password, RoleId (int), Sport (string) }

// LoginRequestDto
{ Email, Password }

// LoginResponseDto
{ Token (JWT string), UserId (Guid), Name, Role }

// AuthResponseDto
{ Id (Guid), RoleId, Name, Email, Sport, Message }
```

**Coach:**
```csharp
// CoachResponseDto
{ Id, FullName, Sport, Bio, HourlyRate, Experience, AverageRating, Location }
```

**LessonPackage:**
```csharp
// CreateLessonPackageDto
{ PackageName, PackageDescription, DurationInMinutes, PackagePrice,
  Requirements (List<string>), LocationType (enum), LessonModel (enum), CoverImageUrl }

// LessonPackageResponseDto
{ Id, CoachId, PackageName, PackageDescription, DurationInMinutes, PackagePrice,
  Requirements (List<string>), LocationType (string), LessonModel (string),
  CoverImageUrl, IsActive }
```

### 4.3 AutoMapper Profiles

**`CoachProfile`:**
```csharp
CreateMap<Coach, CoachResponseDto>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : "Unknown Coach"));
```

**`LessonPackageProfile`:**
```csharp
CreateMap<LessonPackage, LessonPackageResponseDto>()
    .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType.ToString()))
    .ForMember(dest => dest.LessonModel, opt => opt.MapFrom(src => src.LessonModel.ToString()));
```

### 4.4 Custom Exceptions
- `NotFoundException` — caught by `ExceptionHandlingMiddleware`, returns HTTP 404

### 4.5 Dependency Injection (Application)
```csharp
services.AddScoped<ICoachService, CoachService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ILessonPackageService, LessonPackageService>();
services.AddAutoMapper(Assembly.GetExecutingAssembly()); // scans for all Profile classes
```

---

## 5. Infrastructure.Persistence Layer

### 5.1 AppDbContext
```
DbSets: Users, Roles, Coaches, LessonPackages, Reservations, Payments, AuditLogs
OnModelCreating: applies all IEntityTypeConfiguration files from assembly
```

### 5.2 Repository Interfaces & Implementations

**`IGenericRepository<T>` / `GenericRepository<T>`:**
- `AddAsync(T)`, `GetByIdAsync(Guid)`, `GetAllAsync()`, `Update(T)`, `Delete(T)`, `SaveChangesAsync()`

**`IUserRepository` / `UserRepository`:**
- `GetUserByEmailAsync(string email)`
- `IsEmailUniqueAsync(string email)`

**`ICoachRepository` / `CoachRepository`:**
- `GetAllCoachesWithDetailsAsync()` — includes `.Include(c => c.User)`
- `GetCoachByUserIdAsync(Guid userId)`
- `GetCoachesBySportAsync(string sport)` — case-insensitive, only IsApproved
- `GetCoachWithPackagesAsync(Guid coachId)` — includes Packages + User
- `GetTopRatedCoachesAsync(int count)` — ordered by AverageRating desc

**`ILessonPackageRepository` / `LessonPackageRepository`:**
- `GetPackagesByCoachIdAsync(Guid coachId)` — filters by CoachId

**`IReservationRepository` / `ReservationRepository`:**
- (implemented, details in ReservationRepository.cs)

### 5.3 EF Configurations (Fluent API)

**`LessonPackageConfiguration`:**
```csharp
builder.HasKey(p => p.Id);
builder.HasIndex(p => new { p.CoachId, p.PackageName }).IsUnique(); // unique per coach
builder.Property(p => p.PackageName).IsRequired().HasMaxLength(200);
builder.Property(p => p.PackageDescription).HasMaxLength(500);
builder.Property(p => p.CoverImageUrl).HasMaxLength(500);
builder.Property(p => p.PackagePrice).IsRequired().HasPrecision(18, 2);
// Requirements is text[] — EF handles it via Npgsql natively
```

### 5.4 Migrations
- **Single migration**: `20260407122252_InitialCreate` — covers all tables
- Applied automatically on startup via `MigrateAsync()` in `DataBaseSeeder.SeedAsync()`

### 5.5 DataBaseSeeder
- Runs on every startup (inside a scoped `try/catch`)
- First seeds Roles (1=Student, 2=Coach, 3=Admin) if not exist
- Then seeds 2 demo Users + linked Coach profiles if no users exist
- Seed users: "Talha Zeren" (Tennis, Istanbul) and "Serena Williams" (Tennis, LA)

### 5.6 Dependency Injection (Infrastructure)
```csharp
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICoachRepository, CoachRepository>();
services.AddScoped<IReservationRepository, ReservationRepository>();
services.AddScoped<ILessonPackageRepository, LessonPackageRepository>();
services.AddTransient<DataBaseSeeder>();
```

---

## 6. WebApi Layer — `MeetInSport.WebApi`

### 6.1 Controllers & Endpoints

#### `AuthController` — `api/v1/auth` `[Authorize]` at class level

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/auth/register` | `[AllowAnonymous]` | Register new user (Student or Coach) |
| POST | `/api/v1/auth/login` | `[AllowAnonymous]` | Login, returns JWT token |

#### `CoachController` — `api/v1/coaches` `[Authorize]` at class level

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/coaches` | `[AllowAnonymous]` | Get all coaches (with User JOIN) |
| GET | `/api/v1/coaches/{id:guid}` | `[Authorize]` required | Get coach by ID |
| GET | `/api/v1/coaches/sport/{sport}` | `[AllowAnonymous]` | Get coaches by sport (approved only) |

#### `LessonPackageController` — `api/v1/packages` `[Authorize]` at class level

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/packages` | `[Authorize]` required | Create package (CoachId taken from JWT, not body) |
| GET | `/api/v1/packages/coach/{coachId:guid}` | `[Authorize]` required | Get packages by coach ID |

> **Security pattern for CREATE:** `currentUserId` is always read from `ClaimTypes.NameIdentifier` in the JWT — never trusted from the request body. This prevents users from creating packages on behalf of other coaches.

### 6.2 JWT Configuration (`appsettings.json`)
```json
{
  "JwtSettings": {
    "Secret": "MeetInSportSuperSecretKeyForJwtAuthentication2026!",
    "Issuer": "MeetInSportApi",
    "Audience": "MeetInSportClients"
  }
}
```
- **Algorithm:** HmacSha256
- **Expiry:** 7 days
- **ClockSkew:** Zero
- **Validates:** Issuer, Audience, Lifetime, Signing Key

### 6.3 JWT Claims Structure
```csharp
ClaimTypes.NameIdentifier → user.Id (Guid as string)
ClaimTypes.Email          → user.Email
ClaimTypes.Name           → user.Name
ClaimTypes.Role           → user.Role.RoleName  // "Student" | "Coach" | "Admin"
```

### 6.4 Middleware Pipeline (order matters)
```
1. ExceptionHandlingMiddleware   ← catches all unhandled exceptions
2. UseHttpsRedirection
3. UseCors("AllowReactApp")
4. UseAuthentication             ← must come before Authorization
5. UseAuthorization
6. MapControllers
```

### 6.5 ExceptionHandlingMiddleware
- Catches all unhandled exceptions
- Returns `NotFoundException` as HTTP 404 with message
- All other exceptions return HTTP 500 with generic message
- Logs full exception detail via `ILogger`

### 6.6 CORS Policy (`AllowReactApp`)
```csharp
WithOrigins("http://localhost:3000")
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()
```

### 6.7 Swagger Configuration
- Bearer token security definition (Authorization header)
- Security requirement applied globally
- Available at `/swagger` in Development environment

---

## 7. Docker / Infrastructure

### `docker-compose.yml`
```yaml
services:
  db:
    image: postgres:16
    ports: "5433:5432"
    healthcheck: pg_isready
    volumes: postgres_data (persistent)

  api:
    build: ./Dockerfile
    ports: "8080:8080"
    env: ASPNETCORE_URLS=http://+:8080
    depends_on: db (waits for healthy)
    ConnectionStrings: "Host=db;Port=5432;..."
```

- **API runs on port 8080** (no HTTPS in Docker)
- **DB runs on 5433 on host** (5432 inside container)
- Credentials from `.env` file (`POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`)
- DB migrations + seeding run automatically on startup

---

## 8. Known Bugs Fixed (Do Not Re-Introduce)

### Bug 1 — Swagger Authorization header typo
**File:** `Program.cs` — `AddSwaggerGen`
```csharp
// WRONG (was causing 401 on all Swagger calls):
Name = "Authrorization"
// FIXED:
Name = "Authorization"
```

### Bug 2 — `Requirements` must be `List<string>`, NOT `ICollection<string>`
**Files:** `LessonPackage.cs`, `CreateLessonPackageDto.cs`, `LessonPackageResponseDto.cs`

EF Core + Npgsql materializes PostgreSQL `text[]` arrays using `PopulateList<string>()` internally. This method requires `IList<string>`. `ICollection<string>` is **not** assignable to `IList<string>`, causing:
```
ArgumentException: Expression of type 'ICollection`1[String]' cannot be used 
for parameter of type 'IList`1[String]' of method 'PopulateList[String]'
```
**All three types must declare `List<string>` explicitly.**

---

## 9. What Has NOT Been Implemented Yet

The following features exist in the domain/entity model but have **no controller or service implementation yet**:

| Feature | Status | Notes |
|---|---|---|
| Reservation creation | ❌ Not implemented | `ReservationRepository` exists, no service/controller |
| Reservation status management | ❌ Not implemented | Cancel, confirm, complete flows |
| Payment processing | ❌ Not implemented | `Payment` entity exists, no integration |
| Admin approval for coaches | ❌ Not implemented | `IsApproved` flag exists on Coach |
| AuditLog writing | ❌ Not implemented | Entity + DB table exists, not populated |
| Email verification | ❌ Not implemented | `IsEmailVerified` flag exists |
| Coach profile update | ❌ Not implemented | No PATCH/PUT endpoint for coach details |
| Avatar/Cover image upload | ❌ Not implemented | URLs stored as strings only |
| Top-rated coaches endpoint | ❌ Not implemented | `GetTopRatedCoachesAsync` exists in repo |
| Rating system | ❌ Not implemented | `AverageRating` stored but not calculated |
| Frontend (React) | ❌ Not started | API ready on `http://localhost:8080` |

---

## 10. Development Workflow

```bash
# Start everything (DB + API + migrations + seed):
cd MeetInSport
docker compose up --build

# API available at:
http://localhost:8080/swagger

# Run migrations manually (local, not Docker):
dotnet ef migrations add <MigrationName> \
  --project src/MeetInSport.Infrastructure.Persistence \
  --startup-project src/MeetInSport.WebApi

dotnet ef database update \
  --project src/MeetInSport.Infrastructure.Persistence \
  --startup-project src/MeetInSport.WebApi
```

> **IMPORTANT:** Do NOT run migrations inside Docker. The `DataBaseSeeder` calls `MigrateAsync()` on startup which applies all pending migrations automatically against the Docker PostgreSQL container.

---

## 11. Architectural Conventions to Follow

1. **userId always from JWT** — never trust `userId` from the request body on protected endpoints
2. **CoachId resolved server-side** — when a coach creates a package, look up their `Coach` record using `currentUserId` from token
3. **DTOs always for I/O** — entities never exposed directly to controllers
4. **Repository pattern** — controllers only talk to services, services only talk to repositories, repositories only talk to `DbContext`
5. **Clean exception handling** — throw meaningful exceptions from services; middleware handles them centrally
6. **`[AllowAnonymous]`** must be explicitly added to public endpoints since all controllers have `[Authorize]` at class level
7. **`List<string>` for array properties** — never use `ICollection<string>` for EF Core properties mapped to PostgreSQL arrays