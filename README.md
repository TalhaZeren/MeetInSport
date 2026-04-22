# 🏟️ MeetInSport - Note:READMEfile will be updated in next times.

> **A platform that connects sports coaches with students — enabling discovery, booking, and lesson management.**

![Status](https://img.shields.io/badge/status-in%20progress-yellow?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/license-Apache%202.0-green?style=for-the-badge)

---

> [!NOTE]
> **🚧 This project is actively under development.** Core features like authentication, coach management, lesson packages, and reservation booking are implemented and functional. New features and improvements are being added continuously.

---

## 📖 About

**MeetInSport** is a RESTful Web API that serves as the backend for a sports coaching marketplace. The platform allows:

- **Students** to discover coaches, browse lesson packages, and book reservations.
- **Coaches** to register, create customizable lesson packages, and manage incoming bookings.
- **Admins** to oversee the platform (planned).

The project is built with **Clean Architecture** principles, ensuring a clear separation of concerns, testability, and maintainability.

---

## 🏗️ Architecture

The solution follows **Clean Architecture** (also known as Onion Architecture), organized into four distinct layers:

```
MeetInSport.sln
└── src/
    ├── MeetInSport.Domain                        # Core domain entities & enums (no dependencies)
    ├── MeetInSport.Application                   # Business logic, DTOs, interfaces, services
    ├── MeetInSport.Infrastructure.Persistence     # EF Core, repositories, DB config, seeders
    └── MeetInSport.WebApi                         # Controllers, middleware, program entry point
```

### Dependency Flow

```
WebApi → Application → Domain
WebApi → Infrastructure.Persistence → Application → Domain
```

> The **Domain** layer has zero external dependencies. The **Application** layer defines interfaces that are implemented by the **Infrastructure** layer, following the Dependency Inversion Principle.

---

## 🧩 Domain Model

### Entities

| Entity | Description |
|---|---|
| **User** | Platform user with role-based access (Student, Coach, Admin). Supports email/password authentication. |
| **Role** | Defines user types: `Student` (1), `Coach` (2), `Admin` (3). |
| **Coach** | Extended profile linked 1:1 to a User. Contains sport, bio, hourly rate, experience, rating, location, and IBAN. |
| **LessonPackage** | A bookable lesson offering created by a coach. Includes name, description, duration, price, requirements, location type, and lesson model. |
| **Reservation** | A booking made by a student for a specific lesson package. Tracks scheduling, status, and cancellation details. |
| **Payment** | 1:1 relationship with Reservation. Tracks amount, currency (TRY), status, and transaction ID. |
| **AuditLog** | Tracks user actions across the system for auditing purposes. |

### Enums

| Enum | Values |
|---|---|
| `LessonModel` | `OneOnOne`, `Group` |
| `LocationType` | `CoachLocation`, `StudentLocation`, `Online` |
| `ReservationStatus` | `Pending`, `Confirmed`, `Cancelled`, `Completed`, `Refunded` |
| `PaymentStatus` | `Pending`, `Completed`, `Failed`, `Refunded` |

### Entity Relationships

```
User 1──1 Coach (optional, only if role = Coach)
User 1──N Reservation (as Student)
User 1──N AuditLog
Coach 1──N LessonPackage
Coach 1──N Reservation
LessonPackage 1──N Reservation
Reservation 1──1 Payment (optional)
Role 1──N User
```

---

## 🔌 API Endpoints

### Authentication — `api/v1/auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | 🔓 Public | Register a new user (student or coach) |
| `POST` | `/api/v1/auth/login` | 🔓 Public | Login and receive a JWT token |

### Coaches — `api/v1/coaches`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/v1/coaches` | 🔓 Public | List all coaches |
| `GET` | `/api/v1/coaches/{id}` | 🔒 Required | Get coach by ID |
| `GET` | `/api/v1/coaches/sport/{sport}` | 🔓 Public | Filter coaches by sport |

### Lesson Packages — `api/v1/packages`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/packages` | 🔒 Required | Create a new lesson package (coaches only) |
| `GET` | `/api/v1/packages/coach/{coachId}` | 🔒 Required | Get all packages for a specific coach |

### Reservations — `api/v1/reservation`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/reservation` | 🔒 Required | Create a new reservation (students) |

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 8.0 (Web API) |
| **Language** | C# 12 |
| **Database** | PostgreSQL 16 |
| **ORM** | Entity Framework Core 8.0 |
| **Authentication** | JWT Bearer Tokens |
| **Password Hashing** | BCrypt.Net |
| **Object Mapping** | AutoMapper 12 |
| **Validation** | FluentValidation (configured, integration in progress) |
| **Mediator** | MediatR (configured, CQRS integration planned) |
| **API Docs** | Swagger / Swashbuckle |
| **Containerization** | Docker & Docker Compose |
| **Logging** | Serilog (Console Sink) |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker & Docker Compose](https://www.docker.com/get-started)
- [PostgreSQL 16](https://www.postgresql.org/) (or use the Docker container)

### Option 1: Run with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/MeetInSport.git
   cd MeetInSport
   ```

2. **Create a `.env` file** in the project root (next to `docker-compose.yml`):
   ```env
   POSTGRES_USER=your_username
   POSTGRES_PASSWORD=your_password
   POSTGRES_DB=meetinsportDb
   ```

3. **Start the containers**
   ```bash
   docker compose up --build
   ```

4. **Access the API**
   - API: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080/swagger`

### Option 2: Run Locally

1. **Clone and navigate**
   ```bash
   git clone https://github.com/your-username/MeetInSport.git
   cd MeetInSport
   ```

2. **Start PostgreSQL** (via Docker or locally) on port `5433`:
   ```bash
   docker compose up db -d
   ```

3. **Update the connection string** in `src/MeetInSport.WebApi/appsettings.json` if needed.

4. **Run the application**
   ```bash
   dotnet run --project src/MeetInSport.WebApi
   ```

5. **Open Swagger UI** at `https://localhost:{port}/swagger`

### Database Seeding

On first run, the application automatically:
- Applies all pending EF Core migrations
- Seeds **3 roles**: Student, Coach, Admin
- Seeds **2 sample users** with linked coach profiles

---

## 📁 Project Structure

```
MeetInSport/
├── docker-compose.yml
├── Dockerfile
├── LICENSE                          # Apache 2.0
├── MeetInSport.sln
│
└── src/
    ├── MeetInSport.Domain/
    │   ├── Common/
    │   │   └── BaseEntity.cs        # Id, CreatedAt, UpdatedAt, IsDeleted
    │   ├── Entities/
    │   │   ├── User.cs
    │   │   ├── Role.cs
    │   │   ├── Coach.cs
    │   │   ├── LessonPackage.cs
    │   │   ├── Reservation.cs
    │   │   ├── Payment.cs
    │   │   └── AuditLog.cs
    │   └── Enum/
    │       ├── LessonModel.cs
    │       ├── LocationType.cs
    │       ├── PaymentStatus.cs
    │       └── ReservationStatus.cs
    │
    ├── MeetInSport.Application/
    │   ├── DependencyInjection.cs
    │   ├── DTOs/
    │   │   ├── Auth/                # Register & Login request/response DTOs
    │   │   ├── Coach/               # CoachResponseDto
    │   │   ├── LessonPackage/       # Create & Response DTOs
    │   │   └── Reservation/         # Create & Response DTOs
    │   ├── Exceptions/
    │   │   └── NotFoundException.cs
    │   ├── Interface/
    │   │   ├── Repositories/        # IGenericRepository, IUserRepository, etc.
    │   │   └── Services/            # IAuthService, ICoachService, etc.
    │   ├── Mappings/
    │   │   ├── CoachProfile.cs
    │   │   ├── LessonPackageProfile.cs
    │   │   └── ReservationProfile.cs
    │   └── Services/
    │       ├── AuthService.cs
    │       ├── CoachService.cs
    │       ├── LessonPackageService.cs
    │       └── ReservationService.cs
    │
    ├── MeetInSport.Infrastructure.Persistence/
    │   ├── DependencyInjection.cs
    │   ├── AppDbContext.cs
    │   ├── Configurations/          # EF Core Fluent API configurations
    │   │   ├── UserConfiguration.cs
    │   │   ├── RoleConfiguration.cs
    │   │   ├── CoachConfiguration.cs
    │   │   ├── LessonPackageConfiguration.cs
    │   │   ├── ReservationConfiguration.cs
    │   │   ├── PaymentConfiguration.cs
    │   │   └── AuditLogConfiguration.cs
    │   ├── Migrations/
    │   ├── Repositories/
    │   │   ├── GenericRepository.cs
    │   │   ├── UserRepository.cs
    │   │   ├── CoachRepository.cs
    │   │   ├── LessonPackageRepository.cs
    │   │   └── ReservationRepository.cs
    │   └── Seeders/
    │       └── DataBaseSeeder.cs
    │
    └── MeetInSport.WebApi/
        ├── Program.cs               # Application entry point & middleware pipeline
        ├── appsettings.json
        ├── Controllers/
        │   ├── AuthController.cs
        │   ├── CoachController.cs
        │   ├── LessonPackageController.cs
        │   └── ReservationController.cs
        └── Middlewares/
            ├── ExceptionHandlingMiddleware.cs
            └── ErrorDetails.cs
```

---

## ✅ Implemented Features

- [x] Clean Architecture project structure
- [x] Domain entities with base entity (soft delete, timestamps)
- [x] PostgreSQL integration with EF Core (Code-First)
- [x] Fluent API entity configurations
- [x] Generic Repository pattern
- [x] Specialized repositories (User, Coach, LessonPackage, Reservation)
- [x] User registration with role-based profile creation
- [x] JWT authentication & authorization
- [x] Password hashing with BCrypt
- [x] Coach profile management (list all, get by ID, filter by sport)
- [x] Lesson package CRUD (create, list by coach)
- [x] Reservation creation with automatic coach assignment from package
- [x] AutoMapper integration for DTO mapping
- [x] Global exception handling middleware
- [x] Database seeding (roles + sample data)
- [x] Docker & Docker Compose setup
- [x] Swagger UI with JWT Bearer auth support
- [x] CORS configuration for React frontend

---

## 🗺️ Roadmap

Upcoming features and improvements planned for future development:

- [ ] Payment integration and processing
- [ ] Reservation status management (confirm, cancel, complete)
- [ ] Coach approval workflow (Admin)
- [ ] Admin dashboard endpoints
- [ ] Rating & review system for coaches
- [ ] FluentValidation integration for request DTOs
- [ ] CQRS pattern with MediatR
- [ ] Email verification flow
- [ ] Refresh token mechanism
- [ ] Pagination & filtering for list endpoints
- [ ] Unit & integration tests
- [ ] CI/CD pipeline
- [ ] React frontend application
- [ ] Real-time notifications (SignalR)

---

## 🔐 Authentication

The API uses **JWT Bearer tokens** for authentication. To access protected endpoints:

1. **Register** a new account via `POST /api/v1/auth/register`
2. **Login** via `POST /api/v1/auth/login` to receive a JWT token
3. Include the token in the `Authorization` header:
   ```
   Authorization: Bearer <your-jwt-token>
   ```

In **Swagger UI**, click the 🔒 **Authorize** button and enter: `Bearer <your-token>`

---

## 🐳 Docker

The project includes a multi-stage Dockerfile and Docker Compose configuration:

- **`db`** — PostgreSQL 16 container with health checks
- **`api`** — .NET 8.0 runtime container (published in Release mode)

```bash
# Start all services
docker compose up --build

# Start only the database
docker compose up db -d

# Stop all services
docker compose down

# Stop and remove volumes
docker compose down -v
```

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](https://github.com/your-username/MeetInSport/issues).

1. Fork the project
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **Apache License 2.0** — see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  <b>MeetInSport</b> — Built with ❤️ for the sports community
</p>
