using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WestCoastFitness.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` / `dotnet ef database update` construct the
/// DbContext directly, without booting the Api project's full host (JWT
/// signing-key validation, migration-on-startup, etc.). This is the
/// standard pattern for a DbContext that lives outside the startup
/// project. The connection string only needs to be valid enough for EF to
/// generate SQL against the Npgsql provider; it does not need to resolve
/// to a reachable server for `migrations add`.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=westcoastfitness;Username=westcoastfitness;Password=westcoastfitness_dev_only";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
