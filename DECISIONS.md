# FlowCore Decisions

## 2026-06-08: Use ASP.NET Core Minimal API For MVP

The MVP uses ASP.NET Core 8 minimal APIs to keep the backend explicit and compact while still showing domain services, EF Core persistence, Swagger, and testable workflow rules.

## 2026-06-08: Keep Workflow Rules In A Domain Service

Status transition validation, movement creation, forwarding behavior, and audit logging live in `ProtocolWorkflowService`. This keeps endpoints thin and makes core rules straightforward to test.

## 2026-06-08: Store Attachment Metadata Only

The MVP records file name, content type, size, uploader, and timestamp. Binary storage is intentionally out of scope so the first version can focus on workflow, auditability, and relational modeling.

## 2026-06-08: Scaffold Angular Manually

The environment has Node.js and npm but no Angular CLI. The Angular app is represented as a standard standalone Angular project so it can run after `npm install` installs local CLI dependencies.

## 2026-06-08: Verification Limited By Missing .NET SDK

The local environment does not currently provide `dotnet`. API build, EF migrations, and xUnit execution are documented as pending commands in `TASKS.md` until the SDK is installed.

## 2026-06-08: Add Manual Initial Migration

The initial SQL Server schema is represented as an EF Core migration class because `dotnet ef` is not available in the environment. It should be validated or regenerated once the .NET SDK is installed.
