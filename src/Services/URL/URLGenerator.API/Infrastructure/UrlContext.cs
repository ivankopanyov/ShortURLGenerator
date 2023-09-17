namespace ShortURLGenerator.URLGenerator.API.Infrastructure;

/// <summary>Database context.</summary>
public class UrlContext : DbContext
{
    /// <summary>Set of generated short URLs.</summary>
    public DbSet<Models.Url> Urls { get; set; }

    /// <summary>Overriding the method for creating entity models.</summary>
    /// <param name="builder">Model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UrlEntityTypeConfiguration());
    }

    /// <summary>Overriding the database configuration method.</summary>
    /// <param name="options">Database context options builder.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var hostname = Environment.GetEnvironmentVariable("DB_HOST");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = $"Server={hostname};Database=UrlDb;User Id=SA;Password={password};Trusted_Connection=False;TrustServerCertificate=True";
        options.UseSqlServer(connectionString);
    }
}
