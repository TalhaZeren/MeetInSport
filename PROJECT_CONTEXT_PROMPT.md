# MeetInSport — Complete Project Context Prompt

> **Purpose:** Copy-paste this entire document into a new LLM conversation to continue developing the MeetInSport project with full context.

---

## 1. Project Overview

**MeetInSport** is a sports coaching marketplace REST API built with **C# / .NET 8** and **PostgreSQL**. It connects students who want to learn sports with professional coaches. Coaches create lesson packages, students book reservations, and the system handles authentication, authorization, payments (future), and audit logging.

**Current Status:** Backend API is functional with Auth, Coach, LessonPackage, and Reservation CRUD operations. Running via Docker Compose. No frontend yet (CORS configured for React on port 3000).

**Tech Stack:**
- .NET 8 (ASP.NET Core Web API)
- PostgreSQL 16 (via Docker)
- Entity Framework Core 8.0 (Code-First with Migrations)
- AutoMapper 12.0.1
- BCrypt.Net-Next 4.1.0 (password hashing)
- JWT Authentication (System.IdentityModel.Tokens.Jwt 8.17.0)
- Serilog (logging)
- Swagger/Swashbuckle 6.5.0 (API docs)
- Docker & Docker Compose (containerization)

---

## 2. Architecture — Clean Architecture (4 Layers)

```
MeetInSport.sln
└── src/
    ├── MeetInSport.Domain            (Entities, Enums, BaseEntity — no dependencies)
    ├── MeetInSport.Application       (DTOs, Interfaces, Services, Mappings, Exceptions — depends on Domain)
    ├── MeetInSport.Infrastructure.Persistence  (EF Core, DbContext, Repositories, Configs, Seeders, Migrations — depends on Domain + Application)
    └── MeetInSport.WebApi            (Controllers, Middleware, Program.cs — depends on Application + Infrastructure)
```

**Dependency flow:** Domain ← Application ← Infrastructure.Persistence ← WebApi

---

## 3. Domain Layer (`MeetInSport.Domain`)

**No NuGet dependencies.** Target: `net8.0`.

### 3.1 BaseEntity (`Common/BaseEntity.cs`)
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }  // Soft delete flag
}
```

### 3.2 Entities

**User** (extends BaseEntity):
- `Name`, `Email`, `PasswordHash` (string, required)
- `RoleId` (int, FK to Role)
- `PhoneNumber?`, `AvatarUrl?` (optional strings)
- `KvkkAcceptedAt` (DateTime), `LastLoginAt?`, `IsActive` (default true), `IsEmailVerified` (default false)
- Nav: `Role`, `CoachProfile?` (1:1 optional), `Reservations` (1:N), `AuditLogs` (1:N)

**Role** (NOT BaseEntity — uses `int Id`):
- `Id` (int), `RoleName`, `Description?`
- Nav: `Users` (1:N)
- **Seeded values:** 1=Admin, 2=Coach, 3=Student

**Coach** (extends BaseEntity):
- `UserId` (Guid, FK to User — 1:1)
- `Sport`, `Bio?`, `HourlyRate` (decimal), `Experience` (int, years), `IsApproved` (default false), `AverageRating` (decimal), `Location?`, `Iban?`
- Nav: `User`, `Packages` (1:N), `Reservations` (1:N)

**LessonPackage** (extends BaseEntity):
- `CoachId` (Guid, FK to Coach)
- `PackageName`, `PackageDescription`, `DurationInMinutes` (decimal), `PackagePrice` (decimal)
- `Requirements` (List<string>), `LocationType` (enum), `IsActive`, `LessonModel` (enum), `CoverImageUrl?`
- Nav: `Coach`, `Reservations` (1:N)
- **Unique constraint:** (CoachId, PackageName)

**Reservation** (extends BaseEntity):
- `StudentId` (Guid, FK to User), `CoachId` (Guid, FK to Coach), `PackageId` (Guid, FK to LessonPackage)
- `ScheduledAt`, `LocationType` (enum), `Status` (enum, default Pending), `Notes?`
- `CancelledAt?` (nullable DateTime), `CancelReason?`
- Nav: `Student`, `Coach`, `Package`, `Payment?` (1:1 optional)

**Payment** (extends BaseEntity):
- `ReservationId` (Guid, FK to Reservation — 1:1)
- `Amount` (decimal), `Currency` (string, default "TRY"), `Status` (enum, default Pending), `TransactionId?`, `ProcessedAt?`
- Nav: `Reservation`

**AuditLog** (NOT BaseEntity — uses `long Id`):
- `Id` (long), `UserId` (Guid), `Action`, `EntityType`, `EntityId?`, `OldValue?`, `NewValue?`, `IpAddress?`, `Timestamp` (default UtcNow)
- Nav: `User?`

### 3.3 Enums

```csharp
enum LessonModel    { OneOnOne = 1, Group = 2 }
enum LocationType   { CoachLocation = 1, StudentLocation = 2, Online = 3 }
enum PaymentStatus  { Pending = 1, Completed = 2, Failed = 3, Refunded = 4 }
enum ReservationStatus { Pending = 1, Confirmed = 2, Cancelled = 3, Completed = 4, Refunded = 5 }
```

---

## 4. Application Layer (`MeetInSport.Application`)

**NuGet:** AutoMapper 12.0.1, AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1, BCrypt.Net-Next 4.1.0, FluentValidation 12.1.1, MediatR 14.1.0, Microsoft.Extensions.Configuration, Microsoft.Extensions.DependencyInjection.Abstractions, System.IdentityModel.Tokens.Jwt 8.17.0

**Note:** FluentValidation and MediatR are referenced but NOT yet actively used in service implementations. They are installed for future use.

### 4.1 DTOs

**Auth:**
- `RegisterRequestDto`: Name, Email, Password, RoleId (int), Sport
- `AuthResponseDto`: Id, Name, Email, Message, RoleId, Sport
- `LoginRequestDto`: Email, Password
- `LoginResponseDto`: Token, UserId, Name, Role

**Coach:**
- `CoachResponseDto`: Id, FullName (mapped from User.Name), Sport, Bio?, HourlyRate, Experience, AverageRating, Location?
- `UpdateCoachProfileDto`: Sport, Bio?, HourlyRate, Experience, Location?, Iban?

**LessonPackage:**
- `CreateLessonPackageDto`: PackageName, PackageDescription, DurationInMinutes, PackagePrice, Requirements (List<string>), LocationType (enum), LessonModel (enum), CoverImageUrl? — **CoachId is NOT in DTO, taken from JWT**
- `LessonPackageResponseDto`: Id, CoachId, PackageName, PackageDescription, DurationInMinutes, PackagePrice, Requirements, LocationType (string), LessonModel (string), CoverImageUrl?, IsActive

**Reservation:**
- `CreateReservationDto`: PackageId, ScheduleAt, LocationType (enum), Notes?
- `ReservationResponseDto`: Id, PackageId, CoachId, ScheduleAt, Status (string), LocationType (string), Notes?, CreatedtAt (note: typo in property name)
- `CancelReservationDto`: CancelReason?

### 4.2 Interfaces

**Repository Interfaces:**
- `IGenericRepository<T>`: GetByIdAsync(Guid), GetAllAsync(), AddAsync(T), Update(T), Delete(T), SaveChangesAsync()
- `IUserRepository`: + GetUserByEmailAsync(string), IsEmailUniqueAsync(string)
- `ICoachRepository`: + GetAllCoachesWithDetailsAsync(), GetCoachesBySportAsync(string), GetCoachWithPackagesAsync(Guid), GetTopRatedCoachesAsync(int), GetCoachByUserIdAsync(Guid)
- `ILessonPackageRepository`: + GetPackagesByCoachIdAsync(Guid)
- `IReservationRepository`: + GetReservationsByUserIdAsync(Guid), GetReservationsByCoachIdAsync(Guid)

**Service Interfaces:**
- `IAuthService`: RegisterAsync(RegisterRequestDto) → AuthResponseDto, LoginAsync(LoginRequestDto) → LoginResponseDto
- `ICoachService`: GetAllCoachesAsync(), GetCoachByIdAsync(Guid), GetCoachesBySportAsync(string), UpdateProfileAsync(Guid userId, UpdateCoachProfileDto)
- `ILessonPackageService`: CreatePackageAsync(CreateLessonPackageDto, Guid currentUserId), GetPackagesByCoachIdAsync(Guid), DeletePackageAsync(Guid packageId, Guid userId)
- `IReservationService`: CreateReservationAsync(CreateReservationDto, Guid studentId), GetMyReservationsAsync(Guid userId, string role), CancelReservationAsync(Guid reservationId, Guid userId, string role, CancelReservationDto)

### 4.3 Service Implementations

**AuthService:**
- Registration: Checks email uniqueness, hashes password with BCrypt, creates User. If RoleId==2 (Coach), also creates a Coach entity with defaults. Saves to DB.
- Login: Finds user by email (includes Role), verifies BCrypt hash, generates JWT with claims (NameIdentifier, Email, Name, Role). Token expires in 7 days.

**CoachService:** Uses AutoMapper. GetAll includes User details. UpdateProfile finds coach by UserId, updates fields, saves.

**LessonPackageService:** CreatePackage finds coach by userId (from JWT), creates LessonPackage entity. DeletePackage verifies ownership (coach.Id must match package.CoachId). Uses soft delete via AppDbContext interceptor.

**ReservationService:** CreateReservation verifies package exists, creates Reservation with CoachId from package. GetMyReservations returns by CoachId or StudentId based on role. CancelReservation checks authorization (coach ownership or student ownership), validates status is not already Cancelled/Completed, sets status to Cancelled.

### 4.4 AutoMapper Profiles

- `CoachProfile`: Coach → CoachResponseDto, maps `FullName` from `src.User.Name`
- `LessonPackageProfile`: LessonPackage → LessonPackageResponseDto, converts enum `LocationType` and `LessonModel` to strings via `.ToString()`
- `ReservationProfile`: Reservation → ReservationResponseDto, converts enum `Status` and `LocationType` to strings

### 4.5 Custom Exceptions
- `NotFoundException(string message)` / `NotFoundException(string name, object key)` — extends Exception

### 4.6 DI Registration (`DependencyInjection.cs`)
```csharp
services.AddScoped<ICoachService, CoachService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ILessonPackageService, LessonPackageService>();
services.AddScoped<IReservationService, ReservationService>();
services.AddAutoMapper(Assembly.GetExecutingAssembly());
```

---

## 5. Infrastructure.Persistence Layer

**NuGet:** Microsoft.EntityFrameworkCore 8.0.0, Microsoft.EntityFrameworkCore.Relational 8.0.0, Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0

### 5.1 AppDbContext

**Namespace:** `MeetInSport.Infrastructure.Persistance` (note: typo in namespace — "Persistance" instead of "Persistence". This is consistent throughout the project and MUST be preserved.)

- DbSets: Users, Roles, Coaches, LessonPackages, Reservations, Payments, AuditLogs
- `OnModelCreating`: Applies all IEntityTypeConfiguration from assembly. Adds **global query filters** for soft delete on User, Coach, LessonPackage, Reservation, Payment.
- `SaveChangesAsync` override: Intercepts changes to BaseEntity — sets CreatedAt on Added, UpdatedAt on Modified, converts Deleted to Modified with IsDeleted=true and UpdatedAt (soft delete pattern).

### 5.2 EF Configurations (Fluent API)

**UserConfiguration:** PK=Id, Email required/unique/max255, Name required/max100, PhoneNumber max20, AvatarUrl max500. User→Coach: 1:1 via UserId, Cascade delete.

**RoleConfiguration:** PK=Id, RoleName required/max50, Description max200. Role→Users: 1:N via RoleId, Restrict delete. **HasData seed:** Admin(1), Coach(2), Student(3).

**CoachConfiguration:** Sport required/max100, Location max255, Iban max34, Bio max1000, HourlyRate precision(18,2), AverageRating precision(3,2). Coach→Packages: 1:N via CoachId, Cascade delete.

**LessonPackageConfiguration:** PK=Id, Unique index on (CoachId, PackageName), PackageName required/max200, PackageDescription max500, CoverImageUrl max500, PackagePrice required/precision(18,2).

**ReservationConfiguration:** Student relationship: Reservation→User via StudentId, Restrict delete. Coach relationship: Reservation→Coach via CoachId, Restrict delete. Payment: 1:1 via ReservationId, Cascade delete. Package: Reservation→LessonPackage via PackageId, Cascade delete.

**PaymentConfiguration:** PK=Id, Amount required/precision(18,2), Currency required/max3, TransactionId max255.

**AuditLogConfiguration:** PK=Id, Action required/max50, EntityType required/max100, IpAddress required/max45. AuditLog→User via UserId, SetNull on delete.

### 5.3 Repository Implementations

**GenericRepository<T>:** Wraps DbSet. AddAsync, Delete (Remove), GetAllAsync (ToList), GetByIdAsync (FindAsync), SaveChangesAsync, Update (Entry.State=Modified).

**UserRepository:** GetUserByEmailAsync includes Role navigation. IsEmailUniqueAsync checks AnyAsync.

**CoachRepository:** GetCoachesBySportAsync filters by sport (case-insensitive) AND IsApproved. GetCoachWithPackagesAsync includes Packages+User. GetTopRatedCoachesAsync filters IsApproved, orders by AverageRating desc. GetAllCoachesWithDetailsAsync includes User. GetCoachByUserIdAsync filters by UserId.

**LessonPackageRepository:** GetPackagesByCoachIdAsync filters by CoachId.

**ReservationRepository:** GetReservationsByUserIdAsync includes Coach+Package, filters by StudentId. GetReservationsByCoachIdAsync includes Student+Package, filters by CoachId.

### 5.4 DataBaseSeeder

Runs on app startup. Calls `MigrateAsync()` first. Seeds 3 Roles if empty. Seeds 2 sample Users (Talha Zeren & Serena Williams) as coaches with Coach entities if no users exist. Uses dummy password hashes for seed data.

### 5.5 Migrations

1. `20260407122252_InitialCreate` — Initial schema
2. `20260427143417_MakeCancelledAtNullable` — Made `Reservation.CancelledAt` nullable and `Coach.Location` nullable

### 5.6 DI Registration
```csharp
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICoachRepository, CoachRepository>();
services.AddScoped<IReservationRepository, ReservationRepository>();
services.AddScoped<ILessonPackageRepository, LessonPackageRepository>();
services.AddTransient<DataBaseSeeder>();
```

---

## 6. WebApi Layer

**NuGet:** Microsoft.AspNetCore.Authentication.JwtBearer 8.0.10, Microsoft.EntityFrameworkCore.Design 8.0.0, Serilog.AspNetCore 10.0.0, Serilog.Sinks.Console 6.1.1, Swashbuckle.AspNetCore 6.5.0, Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0

### 6.1 Program.cs Pipeline

1. `AddControllers()`
2. `AddDbContext<AppDbContext>` with Npgsql (connection string from config)
3. `AddInfrastructure()` + `AddApplication()` (custom DI extensions)
4. JWT Bearer authentication configured (validates issuer, audience, signing key, lifetime, zero clock skew)
5. CORS policy "AllowReactApp" allows `http://localhost:3000`
6. Swagger with Bearer security definition (padlock icon)
7. **On startup:** Creates scope, runs `DataBaseSeeder.SeedAsync()`
8. Middleware pipeline: `ExceptionHandlingMiddleware` → Swagger (dev only) → HTTPS redirect → CORS → Authentication → Authorization → MapControllers

### 6.2 Controllers

**AuthController** (`api/v1/auth`):
- `[Authorize]` at class level, `[AllowAnonymous]` on register/login
- `POST register` → 201 Created
- `POST login` → 200 OK with JWT token

**CoachController** (`api/v1/coaches`):
- `[Authorize]` at class level
- `GET /` [AllowAnonymous] → all coaches
- `GET /{id:guid}` → coach by ID
- `GET /sport/{sport}` [AllowAnonymous] → coaches by sport
- `PUT /profile` → update own profile (userId from JWT NameIdentifier claim)

**LessonPackageController** (`api/v1/packages`):
- `[Authorize]` at class level
- `POST /` → create package (userId from JWT)
- `GET /coach/{coachId:guid}` → packages by coach
- `DELETE /{id:guid}` → delete package (ownership check, returns 403 on unauthorized)

**ReservationController** (`api/v1/reservation`):
- `[Authorize]` at class level
- `POST /` → create reservation (studentId from JWT)
- `GET /me` → my reservations (role-aware: Coach sees by CoachId, Student sees by StudentId)
- `PUT /{id:guid}/cancel` → cancel reservation (role-aware authorization, returns 403/400)

### 6.3 Middleware

**ExceptionHandlingMiddleware:** Global try-catch. Maps `NotFoundException` → 404, everything else → 500. Returns JSON `ErrorDetails` (StatusCode, Message).

**ErrorDetails:** Simple class with StatusCode and Message, serialized to JSON.

### 6.4 appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=dummy_meetinsportDb;Username=dummy_meetinsportDb;Password=dummy_admin123;"
  },
  "JwtSettings": {
    "Secret": "MeetInSportSuperSecretKeyForJwtAuthentication2026!",
    "Issuer": "MeetInSportApi",
    "Audience": "MeetInSportClients"
  }
}
```
**Note:** The appsettings connection string uses dummy values. The real connection string is injected via Docker Compose environment variable `ConnectionStrings__DefaultConnection`.

---

## 7. Docker Setup

### Dockerfile (multi-stage)
- **Build stage:** `mcr.microsoft.com/dotnet/sdk:8.0`, restores NuGet, publishes WebApi in Release mode
- **Runtime stage:** `mcr.microsoft.com/dotnet/aspnet:8.0`, exposes port 8080, entrypoint `dotnet MeetInSport.WebApi.dll`

### docker-compose.yml
- **db service:** postgres:16, container name `meetinsport_db`, port 5433:5432, persistent volume `postgres_data`, healthcheck with `pg_isready`
- **api service:** builds from Dockerfile, container name `meetinsport_api`, port 8080:8080, depends on db (healthy), injects connection string via env var
- Environment uses `.env` file: `POSTGRES_USER=admin`, `POSTGRES_PASSWORD=admin123`, `POSTGRES_DB=meetinsportDb`

### How to Run
```bash
cd MeetInSport
docker compose up --build
# API available at http://localhost:8080
# Swagger at http://localhost:8080/swagger
```

---

## 8. Database Schema (Entity Relationships)

```
Role (1) ──────< (N) User
User (1) ──────── (0..1) Coach
User (1) ──────< (N) Reservation (as Student)
User (1) ──────< (N) AuditLog
Coach (1) ─────< (N) LessonPackage
Coach (1) ─────< (N) Reservation
LessonPackage (1) ──< (N) Reservation
Reservation (1) ───── (0..1) Payment
```

**Delete behaviors:**
- Role → User: Restrict
- User → Coach: Cascade
- Reservation → Student: Restrict
- Reservation → Coach: Restrict
- Reservation → Payment: Cascade
- Reservation → Package: Cascade
- Coach → Packages: Cascade
- AuditLog → User: SetNull

**Soft delete:** All BaseEntity entities (User, Coach, LessonPackage, Reservation, Payment) use soft delete via IsDeleted flag + global query filters.

---

## 9. Authentication & Authorization Flow

1. User registers via `POST /api/v1/auth/register` (AllowAnonymous)
2. If RoleId=2 (Coach), a Coach entity is auto-created with default values
3. User logs in via `POST /api/v1/auth/login` → receives JWT token
4. JWT contains claims: NameIdentifier (User.Id), Email, Name, Role (RoleName)
5. Protected endpoints extract userId from `ClaimTypes.NameIdentifier` and role from `ClaimTypes.Role`
6. Token expiry: 7 days, validated with issuer/audience/signing key

---

## 10. Known Issues & Technical Debt

1. **Namespace typo:** `MeetInSport.Infrastructure.Persistance` (should be "Persistence") — used consistently, do NOT change without updating all references
2. **DTO typo:** `ReservationResponseDto.CreatedtAt` (extra 't') — would be a breaking change for API consumers if renamed
3. **Seeder role IDs:** DataBaseSeeder seeds roles as Student=1, Coach=2 but RoleConfiguration seeds Admin=1, Coach=2, Student=3. The seeder values differ from configuration seed data. The seeder runs first and takes precedence.
4. **FluentValidation & MediatR:** Installed but not yet used — planned for future validation and CQRS patterns
5. **Payment system:** Entity and configuration exist but no PaymentService/PaymentController implemented yet
6. **AuditLog system:** Entity and configuration exist but no service/controller for creating audit log entries
7. **Coach approval flow:** `IsApproved` field exists but no admin endpoint to approve coaches
8. **Error messages:** Mix of Turkish and English in error messages (e.g., "Bu pakete sahip değilsiniz", "Geçersiz Token Isteği")
9. **No unit tests** — no test project exists yet
10. **No email verification** — `IsEmailVerified` field exists but no verification flow

---

## 11. File Structure Reference

```
MeetInSport/
├── .env                          # POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB
├── Dockerfile                    # Multi-stage build
├── docker-compose.yml            # db (postgres:16) + api services
├── MeetInSport.sln
└── src/
    ├── MeetInSport.Domain/
    │   ├── Common/BaseEntity.cs
    │   ├── Entities/
    │   │   ├── User.cs, Coach.cs, Role.cs
    │   │   ├── LessonPackage.cs, Reservation.cs
    │   │   ├── Payment.cs, AuditLog.cs
    │   └── Enum/
    │       ├── LessonModel.cs, LocationType.cs
    │       ├── PaymentStatus.cs, ReservationStatus.cs
    │
    ├── MeetInSport.Application/
    │   ├── DependencyInjection.cs
    │   ├── Exceptions/NotFoundException.cs
    │   ├── DTOs/
    │   │   ├── Auth/ (RegisterRequestDto, AuthResponseDto, LoginRequestDto, LoginResponseDto)
    │   │   ├── Coach/ (CoachResponseDto, UpdateCoachProfileDto)
    │   │   ├── LessonPackage/ (CreateLessonPackageDto, LessonPackageResponseDto)
    │   │   └── Reservation/ (CreateReservationDto, ReservationResponseDto, CancelReservationDto)
    │   ├── Interface/
    │   │   ├── Repositories/ (IGenericRepository, IUserRepository, ICoachRepository, ILessonPackageRepository, IReservationRepository)
    │   │   └── Services/ (IAuthService, ICoachService, ILessonPackageService, IReservationService)
    │   ├── Mappings/ (CoachProfile, LessonPackageProfile, ReservationProfile)
    │   └── Services/ (AuthService, CoachService, LessonPackageService, ReservationService)
    │
    ├── MeetInSport.Infrastructure.Persistence/
    │   ├── AppDbContext.cs          # Namespace: MeetInSport.Infrastructure.Persistance (typo)
    │   ├── DependencyInjection.cs
    │   ├── Configurations/ (User, Role, Coach, LessonPackage, Reservation, Payment, AuditLog)
    │   ├── Repositories/ (GenericRepository, UserRepository, CoachRepository, LessonPackageRepository, ReservationRepository)
    │   ├── Seeders/DataBaseSeeder.cs
    │   └── Migrations/ (InitialCreate, MakeCancelledAtNullable)
    │
    └── MeetInSport.WebApi/
        ├── Program.cs
        ├── appsettings.json
        ├── Controllers/ (AuthController, CoachController, LessonPackageController, ReservationController)
        └── Middlewares/ (ExceptionHandlingMiddleware, ErrorDetails)
```

---

## 12. API Endpoints Summary

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/auth/register` | Anonymous | Register user (+ auto-create Coach if RoleId=2) |
| POST | `/api/v1/auth/login` | Anonymous | Login, returns JWT |
| GET | `/api/v1/coaches` | Anonymous | List all coaches with User details |
| GET | `/api/v1/coaches/{id}` | Bearer | Get coach by ID |
| GET | `/api/v1/coaches/sport/{sport}` | Anonymous | Search coaches by sport |
| PUT | `/api/v1/coaches/profile` | Bearer | Update own coach profile |
| POST | `/api/v1/packages` | Bearer | Create lesson package (coach only) |
| GET | `/api/v1/packages/coach/{coachId}` | Bearer | Get packages by coach |
| DELETE | `/api/v1/packages/{id}` | Bearer | Delete own package (soft delete) |
| POST | `/api/v1/reservation` | Bearer | Create reservation |
| GET | `/api/v1/reservation/me` | Bearer | Get my reservations (role-aware) |
| PUT | `/api/v1/reservation/{id}/cancel` | Bearer | Cancel reservation (role-aware) |

---

## 13. Important Design Decisions

1. **Soft Delete Pattern:** All BaseEntity entities use `IsDeleted` flag. EF Core global query filters automatically exclude deleted records. The `SaveChangesAsync` override intercepts `EntityState.Deleted` and converts it to a Modified update with `IsDeleted=true`.

2. **Coach auto-creation on registration:** When a user registers with RoleId=2, a Coach entity is automatically created with default placeholder values (Sport from request, other fields set to defaults).

3. **UserId from JWT (not request body):** All protected endpoints extract the current user's ID from the JWT `NameIdentifier` claim, never from the request body. This prevents users from impersonating others.

4. **Role-aware queries:** Reservation endpoints check the user's role claim to determine whether to query by CoachId or StudentId.

5. **Ownership validation:** Delete/Cancel operations verify that the requesting user owns the resource (package belongs to their coach profile, reservation belongs to them).

---

*Last updated: 2026-04-28*
