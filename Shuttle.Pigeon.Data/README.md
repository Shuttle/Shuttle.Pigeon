# Database updates

Revert to migration name:

```
dotnet ef database update <previous-migration-name>
```

Remove initial update / drop database:

```
dotnet ef database drop --force
```

# Migrations

Remove last migration:

```
dotnet ef migrations remove
```

Run the following in the root of the project to deploy the database using a `command prompt`:

```cmd
dotnet ef database update --connection "Server=<server>;Database=<database>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=<ActiveDirectoryDefault|ActiveDirectoryInteractive>;"
```