# 15 — Setup and Runbook

## Prerequisites

- Windows recommended.
- Visual Studio 2022 or later.
- .NET 8 SDK or later.
- SQL Server 2019 or later.
- SQL Server Management Studio.
- Git.

## Database setup

1. Open SSMS.
2. Execute `db/MangaWorkflowDB_v2_demo_ready.sql`.
3. Execute `db/MangaWorkflowDB_v3_extra_seed_demo_data.sql`.
4. Verify:

```sql
USE MangaWorkflowDB;
SELECT COUNT(*) AS SeriesCount FROM dbo.Series;
SELECT COUNT(*) AS TaskCount FROM dbo.ProductionTasks;
SELECT COUNT(*) AS NotificationCount FROM dbo.Notifications;
```

## Web app setup

Connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

LocalDB alternative:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Run commands

Build:

```bash
dotnet build
```

Run web:

```bash
dotnet run --project MangaWorkflow.Web
```

Run worker:

```bash
dotnet run --project MangaWorkflow.Worker
```

## Troubleshooting

### Cannot connect to SQL Server

Check server name. Try `.` or `(localdb)\MSSQLLocalDB`.

### Trust certificate error

Add:

```text
TrustServerCertificate=True
```

### EF scaffold command not found

Install tool:

```bash
dotnet tool install --global dotnet-ef
```

### Duplicate seed data

The v3 script checks for `Neon Samurai`. If it exists, it will not duplicate data.

### Views not visible in scaffold

EF Core may not scaffold SQL views depending on options. You can query views using keyless entity types or direct raw SQL. It is acceptable to build dashboard from base tables if views are inconvenient.

## Demo reset

For clean demo:

1. Re-run v2.
2. Re-run v3.
3. Start Web.
4. Start Worker or use Admin manual run button.
