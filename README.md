# WarehouseExecution

`WarehouseExecution` is a small warehouse execution prototype built with `.NET 10`, `PostgreSQL`, REST, and gRPC.

The goal of the project is not to model a full WMS, but to demonstrate a coherent backend slice:
- reference data integrity for warehouse locations
- explicit job lifecycle transitions
- separation between public API, application logic, persistence, and worker execution
- pragmatic prototype trade-offs documented in code and README

## What The Prototype Does

The system models a warehouse job that moves product from one location to another.

Current flow:
1. A client creates a job through REST using `fromLocation` and `toLocation` codes.
2. The API resolves those codes against the `Locations` reference table.
3. The job is stored in PostgreSQL with `FromLocationId` and `ToLocationId` foreign keys.
4. A gRPC worker can execute the job.
5. Execution creates exactly one `JobStep` in the current prototype.
6. The worker moves the job through `Created -> Planned -> InProgress -> Completed`.
7. A `Created` job can be cancelled directly.
8. An `InProgress` job can be cancelled through the worker flow.

Current prototype restriction:
- one `Job` has exactly one `JobStep`

## Architecture

The solution is split into five projects.

- `Domain`
  Core entities and enums: `Job`, `JobStep`, `Location`, `JobStatus`, `JobStepStatus`.

- `Application`
  Use cases, query models, and application-level validation/errors.

- `Infrastructure`
  EF Core persistence, migrations, repositories, and the database-backed `JobNumber` generator.

- `Api`
  Public REST API for reading locations and creating/reading jobs.

- `Worker`
  Internal gRPC service responsible for job execution and cancellation scheduling.

## Key Decisions

### 1. Locations Are Reference Data, Not Free Strings

The project originally used string fields for `fromLocation` and `toLocation`.

That was replaced with:
- `Locations` reference table
- `unique(code)`
- `Job.FromLocationId` / `Job.ToLocationId`
- `JobStep.FromLocationId` / `JobStep.ToLocationId`

The public API still accepts location codes, but the database stores foreign keys.

Why:
- prevents invalid location values from being persisted
- keeps API ergonomic while enforcing referential integrity
- makes domain rules around routing stricter and easier to evolve

### 2. API Contracts Are Separate From Domain Entities

The API does not return EF/domain entities directly.

Read endpoints return dedicated DTOs with location codes rather than raw foreign keys.

Why:
- avoids leaking persistence concerns into the HTTP contract
- keeps the external model stable if the internal model changes
- makes the API easier to read and test

### 3. Errors Are Explicit

Application logic uses explicit exception types:
- `ValidationException`
- `NotFoundException`
- `ConflictException`

The API maps them to:
- `400`
- `404`
- `409`

Why:
- makes business failures predictable
- removes generic `500` behavior for expected scenarios
- gives cleaner boundaries between application logic and transport

### 4. Execution Is Intentionally Prototype-Grade

The worker uses an in-memory scheduler for delayed execution completion.

Why:
- it keeps the prototype focused on lifecycle orchestration
- it avoids adding a queue or durable scheduler before the domain slice is stable

Trade-off:
- scheduled waiting is not durable across worker restarts

In production, this would move to a queue, broker, or durable scheduling mechanism.

### 5. Migrations Run Automatically In The API

For demo convenience, the `Api` host applies pending EF Core migrations on startup.

Why:
- simplest local setup
- one command to get the system running

Trade-off:
- not the best production deployment pattern

In production, schema migration should be handled by a dedicated deployment step or migration job.

## Data Model

### Locations

Reference table:
- `Id`
- `Code`
- `Name`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Seeded locations:
- `A-01`
- `A-02`
- `B-01`
- `B-02`
- `P-01`

### Jobs

Main fields:
- `Id`
- `JobNumber`
- `Status`
- `ProductCode`
- `ProductName`
- `FromLocationId`
- `ToLocationId`
- `CreatedAtUtc`
- `UpdatedAtUtc`

`JobNumber` is a business identifier in the format:

```text
JOB-yyyyMMdd-000001
```

It is generated using a PostgreSQL-backed counter table.

### JobSteps

Main fields:
- `Id`
- `JobId`
- `StepNumber`
- `Status`
- `FromLocationId`
- `ToLocationId`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## REST API

Base URL:

```text
http://localhost:8080
```

Endpoints:
- `GET /locations`
- `GET /jobs`
- `GET /jobs/{id}`
- `POST /jobs`

Create job example:

```http
POST /jobs
Content-Type: application/json

{
  "fromLocation": "A-01",
  "toLocation": "B-01",
  "productCode": "SKU-100",
  "productName": "Box"
}
```

Validation rules currently implemented:
- `fromLocation` is required
- `toLocation` is required
- source and destination must be different
- both location codes must exist in `Locations`

## gRPC Worker

Base address:

```text
http://localhost:8081
```

Proto file:
- [job_execution.proto](/E:/source/WarehouseExecution/Worker/Protos/job_execution.proto)

Methods:
- `ExecuteJob`
- `CancelJob`

The worker translates application exceptions into gRPC statuses:
- invalid input -> `InvalidArgument`
- missing entity -> `NotFound`
- invalid lifecycle transition -> `FailedPrecondition`

## Local Run

### Docker

Start the full stack:

```powershell
docker compose up --build -d
```

Stop containers:

```powershell
docker compose down
```

Stop containers and remove database volume:

```powershell
docker compose down -v
```

### Local Development

Build:

```powershell
dotnet build WarehouseExecution.sln
```

Test:

```powershell
dotnet test WarehouseExecution.sln
```

Run API:

```powershell
dotnet run --project Api
```

Run Worker:

```powershell
dotnet run --project Worker
```

### Database Access

Inside Docker Compose, services use:

```text
Host=postgres;Port=5432;Database=warehouse_exec_db;Username=warehouse_exec_user;Password=warehouse_exec_pass
```

From the host machine:

```text
localhost:55432
```

## Test Coverage

The solution currently includes unit tests for:
- `GET /locations`
- `GET /jobs`
- `GET /jobs/{id}`
- `POST /jobs`
- application-level validation for job creation
- cancellation of `Created` jobs
- API exception mapping for `400/404/409`

## What Is Deliberately Not Built Yet

This is still a narrow prototype. It does not yet include:
- durable scheduling
- multi-step execution planning
- optimistic concurrency tokens
- integration tests with full HTTP host + database
- richer warehouse rules such as zone compatibility, capacity, or task batching

Those would be the next steps if the prototype were pushed toward a more production-like slice.
