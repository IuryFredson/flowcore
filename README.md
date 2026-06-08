# FlowCore

FlowCore is a corporate protocol and workflow management system for internal administrative teams. It models protocol creation, status transitions, forwarding between departments, movement history, audit logs, and attachment metadata.

## Stack

- Backend: ASP.NET Core 8 minimal API
- Database: SQL Server 2022
- ORM: Entity Framework Core
- Frontend: Angular 18 standalone application
- Tests: xUnit with EF Core InMemory for domain workflow rules
- Infra: Docker Compose for SQL Server

## Repository Layout

```text
apps/
  api/
    src/FlowCore.Api/
    tests/FlowCore.Api.Tests/
  web/
docs/
infra/
docker-compose.yml
```

## Local Setup

1. Start SQL Server:

```bash
docker compose up -d sqlserver
```

2. Run the API:

```bash
cd apps/api/src/FlowCore.Api
dotnet restore
dotnet ef database update
dotnet run
```

3. Run API tests:

```bash
dotnet test FlowCore.sln
```

4. Run the Angular app:

```bash
cd apps/web
npm install
npm start
```

## API Surface

- `GET /health`
- `GET /protocols`
- `GET /protocols/{id}`
- `POST /protocols`
- `POST /protocols/{id}/transitions`
- `POST /protocols/{id}/forward`
- `POST /protocols/{id}/attachments`

Swagger is available in development at `/swagger`.

## Domain Rules

- Closed protocols cannot be forwarded or receive attachment metadata.
- Status transitions must follow the allowed workflow.
- Every status transition creates a movement record.
- Forwarding records origin department, destination department, actor, note, and timestamp.
- Important actions create audit records.

## GitHub

Suggested repository:

```bash
gh repo create IuryFredson/flowcore --public --source . --remote origin --push
```

