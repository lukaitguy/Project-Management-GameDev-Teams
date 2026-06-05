# Web Application for Project Management of Game Development Teams

A full-stack web application for managing game development projects, built as a thesis project using ASP.NET Core 8 and Angular 19. The system supports multi-role team collaboration, task tracking, resource management, build versioning, and project reporting — all tailored to the workflow of a game development studio.

---

## Overview

Game development teams involve multiple disciplines working in parallel — programmers, artists, designers, producers. This application provides a centralized platform where project managers can plan and monitor projects while team members manage their assigned tasks and resources.

The system is organized around three main entities:

- **Projects** — game projects with metadata such as engine, platform, development phase, budget, and genre
- **Tasks** — work items within a project, assigned to team members with priority and status tracking
- **Resources** — assets and tools (software licenses, hardware, etc.) assigned to team members

---

## Features

### User Roles

The system uses ASP.NET Identity with three roles, each with different access levels:

| Role | Description |
|---|---|
| **Administrator** | Full access: manages all users, can assign roles, oversees the entire system |
| **Project Manager** | Creates and manages projects, assigns team members and tasks, views reports |
| **User** | Views assigned projects, manages their own tasks and resources |

### Project Management
- Create, edit, and delete game development projects
- Attach genres, set engine, platform, development phase, budget, and version
- View all project members and their roles within the project

### Task Management
- Create tasks with title, description, priority (Low / Medium / High), and status
- Assign tasks to specific team members
- Comment on tasks for team communication
- Filter and view tasks by project or by assigned user

### Resource Management
- Add resources (e.g., software licenses, hardware) with type and cost
- Assign resources to team members
- Track resources per project

### Build Management
- Log game builds with version number, build type (Alpha / Beta / Release), and patch notes
- View full build history per project

### Reports (Project Managers only)
- Visual reports per project: task completion rates, resource usage, build history overview

### Admin Panel (Administrators only)
- View all registered users
- Create new users and assign roles
- Edit or delete user accounts

---

## Tech Stack

**Backend**
- ASP.NET Core 8
- Entity Framework Core 9 (Code-First, two DB contexts)
- ASP.NET Core Identity
- JWT Bearer Authentication

**Frontend**
- Angular 19 (standalone components)
- TypeScript 5.7
- SCSS (no external UI component library)

**Database**
- Microsoft SQL Server (local via Windows Authentication)

**Tools**
- Visual Studio / Visual Studio Code
- SQL Server Management Studio (SSMS)
- Postman
- Git / GitHub

---

## Architecture

The backend follows a layered architecture:

```
Controllers (API)
    └── Services (business logic)
            └── Repositories (data access via EF Core)
                    └── DbContext → SQL Server
```

Two separate EF Core DbContexts are used:

- **PMDbContext** → `ProjektniMenadzment` database — all application data (projects, tasks, users, resources, builds, comments)
- **PMAuthDbContext** → `ProjektniMenadzmentAuthDb` database — ASP.NET Identity tables (users, roles, claims)

The Angular frontend communicates with the backend exclusively through REST API calls. Route guards (`authGuard`, `adminGuard`, `managerGuard`, `guestGuard`) protect views based on the user's role decoded from the JWT token.

---

## Prerequisites

Before running the application, make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 18+](https://nodejs.org/) and npm
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition is sufficient)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

---

## Setup & Running the Application

### 1. Clone the repository

```bash
git clone https://github.com/lukaitguy/Project-Management-GameDev-Teams.git
cd Project-Management-GameDev-Teams
```

### 2. Configure the database connection

Open `ProjektniMenadzment/appsettings.json` and update the connection strings to match your SQL Server instance:

```json
"ConnectionStrings": {
  "PMConnectionString": "Server=YOUR_SERVER\\SQLEXPRESS;Database=ProjektniMenadzment;Trusted_Connection=true;TrustServerCertificate=Yes;",
  "PMAuthDbConnectionString": "Server=YOUR_SERVER\\SQLEXPRESS;Database=ProjektniMenadzmentAuthDb;Trusted_Connection=true;TrustServerCertificate=Yes;"
}
```

Replace `YOUR_SERVER` with your machine name or SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, `MYPC\SQLEXPRESS`).

### 3. Create the two databases

Navigate to the backend project folder and run EF Core migrations to create both databases:

```bash
cd ProjektniMenadzment

dotnet ef database update --context PMDbContext
dotnet ef database update --context PMAuthDbContext
```

This will create:
- `ProjektniMenadzment` — the main application database with all tables
- `ProjektniMenadzmentAuthDb` — the identity database with ASP.NET Identity tables (users, roles, claims)

> **Alternative:** If you prefer to create the main database schema using a SQL script, run `SKRIPTA_PM.sql` in SSMS against the `ProjektniMenadzment` database. Still run the migration for the auth database.

### 4. Seed the administrator user

After both databases are created, open SSMS and run the `initial_user.sql` script against the `ProjektniMenadzment` database. This inserts the admin profile row linked to the pre-seeded Identity user.

```sql
-- Run this in SSMS against ProjektniMenadzment
-- File: initial_user.sql (in the root of the repository)
```

**Default admin credentials:**

| Field | Value |
|---|---|
| Email | `administrator@pmdb.com` |
| Password | `Admin123!` |

### 5. Run the backend

From the `ProjektniMenadzment` folder:

```bash
dotnet run
```

The API will be available at `http://localhost:5217` (or `https://localhost:7206` for HTTPS).

### 6. Run the frontend

Open a new terminal, navigate to the Angular project, install dependencies, and start the dev server:

```bash
cd ProjektniMenadzment/ClientApp/clientapp
npm install
ng serve
```

The application will be available at `http://localhost:4200`.

---

## Default Login

Once both the backend and frontend are running, open `http://localhost:4200` in your browser and log in with the admin account:

- **Email:** `administrator@pmdb.com`
- **Password:** `Admin123!`

From the admin panel you can create additional users and assign them the **Project Manager** or **User** role.

---

## Project Structure

```
Project-Management-GameDev-Teams/
├── ProjektniMenadzment/
│   ├── Controllers/
│   │   └── Api/                    # REST API controllers
│   ├── Data/
│   │   ├── PMDbContext.cs          # Main app DbContext
│   │   ├── PMAuthDbContext.cs      # Identity DbContext
│   │   └── Migrations/             # EF Core migrations (PM/ and Auth/)
│   ├── Models/
│   │   ├── Domain/                 # Database entity classes
│   │   └── DTOs/                   # Request/response data transfer objects
│   ├── Repositories/               # Data access layer
│   ├── Services/                   # Business logic layer
│   ├── ClientApp/clientapp/        # Angular 19 frontend
│   │   └── src/app/
│   │       ├── features/           # Feature modules (auth, dashboard)
│   │       ├── core/               # Guards, interceptors, services
│   │       ├── layout/             # Layout components
│   │       └── shared/             # Shared UI components
│   ├── wwwroot/app/                # Angular production build output
│   ├── appsettings.json
│   └── Program.cs
├── SKRIPTA_PM.sql                  # Alternative SQL schema script
├── initial_user.sql                # Admin user seed script
└── README.md
```

---

## Database Schema

### Main Database (`ProjektniMenadzment`)

| Table | Description |
|---|---|
| `Projekti` | Game projects |
| `Zadaci` | Tasks within projects |
| `Korisnici` | User profiles (linked to Identity) |
| `Resursi` | Resources assigned to users/projects |
| `Buildovi` | Game build versions |
| `ClanoviProjekta` | Project membership (user ↔ project with role) |
| `KomentariZadatak` | Comments on tasks |
| `Zanrovi` | Game genres |
| `ProjektiZanrovi` | Many-to-many: projects ↔ genres |

### Auth Database (`ProjektniMenadzmentAuthDb`)

Standard ASP.NET Identity tables: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, etc.
