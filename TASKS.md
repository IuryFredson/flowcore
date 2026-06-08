# FlowCore Tasks

## Completed

- Initialized FlowCore repository on `feature/flowcore-mvp`.
- Added ASP.NET Core API project structure.
- Modeled protocol workflow domain entities.
- Added EF Core SQL Server context configuration.
- Added protocol creation, transition, forwarding, listing, detail, and attachment metadata endpoints.
- Added xUnit tests for status transition rules and closed protocol mutation protection.
- Added Angular frontend scaffold for the protocol dashboard.
- Added Docker Compose for SQL Server.
- Added initial EF Core migration for SQL Server schema.
- Added README, SPEC, TASKS, and DECISIONS.
- Created GitHub repository `IuryFredson/flowcore` and pushed `feature/flowcore-mvp`.

## Pending

- Install .NET SDK locally and run `dotnet restore`, `dotnet test FlowCore.sln`, and `dotnet ef database update`.
- Validate the manual initial migration with `dotnet ef migrations list --project apps/api/src/FlowCore.Api`.
- Install Angular dependencies and run `npm run build` in `apps/web`.
- Add seed data endpoint or startup seeding for demo usage.
- Create a `main` base branch and open a PR after SDK/build verification. The GitHub repository currently has `feature/flowcore-mvp` as its default branch because it was created from the feature branch.
