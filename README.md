# Sales API & Frontend

A full-stack solution for managing sales records: a .NET 8 REST API with PostgreSQL and Redis, plus an Angular 21 web UI. The backend follows Domain-Driven Design (DDD) with MediatR, FluentValidation, and EF Core. The frontend provides CRUD, cancellation, filtering, pagination, and live discount previews.

The original challenge brief is preserved in [`README_DEVELOPER_EVALUATION.md`](README_DEVELOPER_EVALUATION.md).

---

## Tech stack

| Layer          | Technologies                                                                    |
| -------------- | ------------------------------------------------------------------------------- |
| **Backend**    | .NET 8, ASP.NET Core, MediatR, FluentValidation, AutoMapper, EF Core, Serilog   |
| **Database**   | PostgreSQL 16                                                                   |
| **Cache**      | Redis 7                                                                         |
| **Frontend**   | Angular 21, Angular Material, RxJS                                              |
| **Tests**      | xUnit, FluentAssertions, NSubstitute                                            |
| **Containers** | Docker Compose                                                                  |

---

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (see `backend/global.json` for the pinned version)
- [Node.js 22+](https://nodejs.org/) and npm (Angular CLI is a dev dependency)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended for the full stack)
- Optional for local backend development without Docker: PostgreSQL 16 and Redis 7

---

## Quick start (Docker)

The fastest way to run everything is from the `backend/` folder:

```bash
cd backend
cp .env.example .env          # Linux/macOS — on Windows: copy .env.example .env
docker compose up --build
```

Apply database migrations **once** (with Postgres running):

```bash
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

| Service        | URL                           | Notes                                             |
| -------------- | ----------------------------- | ------------------------------------------------- |
| **Frontend**   | http://localhost:4200         | Angular app; nginx proxies `/api/` to the API     |
| **API**        | http://localhost:8080         | Direct API access                                 |
| **Swagger UI** | http://localhost:8080/swagger | Available in Development                          |
| **PostgreSQL** | `localhost:5433`              | Host port 5433 → container 5432                   |
| **Redis**      | `localhost:6379`              | Password-protected                                |

### Default credentials

Copy `backend/.env.example` to `backend/.env` before starting Docker. Defaults:

| Variable            | Default value                                 |
| ------------------- | --------------------------------------------- |
| `POSTGRES_DB`       | `developer_evaluation`                        |
| `POSTGRES_USER`     | `developer`                                   |
| `POSTGRES_PASSWORD` | `ev@luAt10n`                                  |
| `REDIS_PASSWORD`    | `ev@luAt10n`                                  |
| `JWT_SECRET_KEY`    | *(see `.env.example` - at least 32 bytes)*    |

> **Security:** `.env` is git-ignored. Never commit real secrets. The values above are for local evaluation only.

Stop the stack:

```bash
docker compose down
```

Remove persisted Postgres data:

```bash
docker compose down -v
```

---

## Local development (without Docker)

### Backend

1. Start PostgreSQL (port **5433**) and Redis locally, or run only those services via Docker:

   ```bash
   cd backend
   docker compose up ambev.developerevaluation.database ambev.developerevaluation.cache
   ```

2. Connection strings in `backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` already target `localhost:5433` and `localhost:6379` with the default credentials.

3. Apply migrations (same command as above).

4. Run the API:

   ```bash
   cd backend
   dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
   ```

   Swagger: http://localhost:8080/swagger (Development environment).

### Frontend

```bash
cd frontend
npm install
npm start
```

Open http://localhost:4200. The dev server proxies `/api` requests to `http://localhost:8080` via `proxy.conf.json`.

---

## Running tests

From the `backend/` folder:

```bash
dotnet test
```

This runs **85 unit tests** covering domain rules (tiered discounts, sale lifecycle), application handlers (CRUD, cancellation, caching), and template validators. Integration and functional test projects exist as scaffolding but are not part of the automated suite.

---

## Database

**PostgreSQL** was chosen because it is production-grade, aligns with the provided template, and supports rich querying for paginated/filtered sales lists (including `ILIKE` wildcard filters). EF Core migrations live in `backend/src/Ambev.DeveloperEvaluation.ORM/Migrations/`.

Create a new migration after model changes:

```bash
dotnet ef migrations add <Name> \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

---

## Configuration

| Source                                                          | Purpose                                                  |
| --------------------------------------------------------------- | -------------------------------------------------------- |
| `backend/.env.example`                                          | Template for Docker Compose variables (copy to `.env`)   |
| `backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` | Local connection strings and JWT secret                  |

Key settings:

- **`ConnectionStrings:DefaultConnection`** — PostgreSQL
- **`ConnectionStrings:Redis`** — Redis (includes password)
- **`Jwt:SecretKey`** — JWT signing key (used by template Auth scaffolding)

---

## API endpoints

Base path: `/api/sales`

| Method   | Path                                      | Description                                   |
| -------- | ----------------------------------------- | --------------------------------------------- |
| `GET`    | `/api/sales`                              | Paginated, sorted, filtered list              |
| `GET`    | `/api/sales/{id}`                         | Get sale by ID                                |
| `POST`   | `/api/sales`                              | Create sale (discounts applied automatically) |
| `PUT`    | `/api/sales/{id}`                         | Update sale                                   |
| `DELETE` | `/api/sales/{id}`                         | Permanently delete sale                       |
| `POST`   | `/api/sales/{id}/cancel`                  | Cancel entire sale                            |
| `POST`   | `/api/sales/{id}/items/{itemId}/cancel`   | Cancel a single line item                     |

**List query parameters** (`GET /api/sales`):

- Pagination: `_page` (default 1), `_size` (default 10, max 100)
- Sorting: `_order` (e.g. `saleDate desc, saleNumber asc`)
- Filters: `customerName` / `customer`, `branchName` / `branch` (supports `*` wildcard), `cancelled`, `customerId`, `branchId`, `_minTotalAmount`, `_maxTotalAmount`, `_minDate`, `_maxDate`

**Discount rules** (per product line, by quantity):

| Quantity | Discount       |
| -------- | -------------- |
| 1-3      | 0%             |
| 4-9      | 10%            |
| 10-20    | 20%            |
| > 20     | Rejected (400) |

**Error format:** `{ "type", "error", "detail" }`

**Health checks:** `/health`, `/health/live`, `/health/ready`

### Swagger & Postman

- **Swagger UI:** http://localhost:8080/swagger (when running in Development)
- **Postman collection:** [`doc/sales-api.postman_collection.json`](doc/sales-api.postman_collection.json) — import into Postman or Insomnia; covers all Sales endpoints with success and error scenarios. See [`doc/README.md`](doc/README.md) for details.

---

## Redis caching

Sales read operations use a **read-through cache** backed by Redis:

| Cache       | Key pattern                        | TTL        | Invalidated when                     |
| ----------- | ---------------------------------- | ---------- | ------------------------------------ |
| Single sale | `sales:item:{id}`                  | 5 minutes  | Any write on that sale               |
| Sale list   | `sales:list:{query fingerprint}`   | 60 seconds | Any sale create/update/delete/cancel |

Every write handler (create, update, delete, cancel sale, cancel item) removes the affected item key and clears all list entries (`sales:list:*`) so stale pages cannot be served.

---

## Angular frontend

The UI under `frontend/src/app/features/sales/` provides:

- Sales list with filters, pagination, and column sorting
- Create / edit forms with reactive validation and live discount previews
- Sale detail view with cancel-sale and cancel-item actions
- Confirm dialogs, loading indicator, and unsaved-changes guard

In Docker, the frontend container serves the production build and proxies API calls. During local development, `ng serve` uses the proxy configuration instead.

---

## Template scaffolding (Users & Auth)

The repository includes **Users** and **Auth** endpoints from the original evaluation template (`/api/users`, `/api/auth`). These are **not part of the Sales challenge scope** — they are kept as reference scaffolding. The Sales API and Angular UI do **not** require authentication.

JWT configuration is wired (`Jwt:SecretKey`, `AddJwtAuthentication`) so a future login flow can be added without restructuring the project.

---

## Implemented vs. future improvements

### Implemented

- Full Sales CRUD with domain-driven design and validation
- Tiered quantity discounts enforced in the domain layer
- Sale and line-item cancellation (history preserved; cancelled items excluded from totals)
- Paginated, sorted, and filtered listing with wildcard name search
- Domain events (`SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`) published via structured logging
- Redis read-through caching with write invalidation
- OpenAPI/Swagger documentation with XML comments
- Angular Material UI with CRUD, filters, and discount previews
- Docker Compose stack (API, frontend, PostgreSQL, Redis)
- 85 automated unit tests

### Future improvements (given more time)

- **JWT-based login and authentication** — wire the existing Auth/Users scaffolding into the Angular app, protect API endpoints, and issue tokens on successful login
- Integration and end-to-end tests against a real database
- Message broker integration for domain events (e.g. RabbitMQ) instead of log-only publishing
- API rate limiting and structured correlation IDs across frontend and backend
- Role-based authorization for sales operations

---

## Known limitations

### AutoMapper NU1903 (GHSA-rvv3-g6hj-g44x)

The backend inherits **AutoMapper 13.0.1** from the evaluation template. NuGet reports a high-severity advisory (GHSA-rvv3-g6hj-g44x) involving stack exhaustion when mapping extremely deep self-referential object graphs (~30k nesting levels).

**Decision:** keep 13.0.1 and suppress the audit warning in the Application project. Patched releases (15+) require a commercial license; this challenge uses AutoMapper only for shallow DTO mapping of Sales entities (no deep or cyclic graphs from API input). The practical risk for this API surface is negligible.

**Mitigation path with more time:** migrate to a maintained fork (e.g. Mapperly or a patched OSS fork), or upgrade under a valid AutoMapper license.

---

## Project structure

```
├── backend/
│   ├── src/
│   │   ├── Ambev.DeveloperEvaluation.Domain/       # Entities, value objects, domain services
│   │   ├── Ambev.DeveloperEvaluation.Application/  # MediatR handlers, DTOs, cache logic
│   │   ├── Ambev.DeveloperEvaluation.ORM/          # EF Core DbContext, migrations, repositories
│   │   ├── Ambev.DeveloperEvaluation.WebApi/       # REST controllers, middleware, Swagger
│   │   ├── Ambev.DeveloperEvaluation.Common/       # Shared utilities (caching, logging, health)
│   │   └── Ambev.DeveloperEvaluation.IoC/          # Dependency injection registration
│   ├── tests/Ambev.DeveloperEvaluation.Unit/       # Unit tests
│   ├── docker-compose.yml
│   └── .env.example
├── frontend/                                       # Angular 21 application
├── doc/                                            # Postman collection and supplementary docs
└── README_DEVELOPER_EVALUATION.md                  # Original challenge brief
```

---

## Additional documentation

- [`doc/README.md`](doc/README.md) — supplementary evaluator docs
- [`doc/sales-api.postman_collection.json`](doc/sales-api.postman_collection.json) — API request collection
- [`README_DEVELOPER_EVALUATION.md`](README_DEVELOPER_EVALUATION.md) — original challenge requirements
