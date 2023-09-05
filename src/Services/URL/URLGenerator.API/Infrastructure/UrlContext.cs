namespace ShortURLGenerator.URLGenerator.API.Infrastructure;

public class UrlContext : DbContext
{
    public DbSet<Url> Urls { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UrlEntityTypeConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var hostname = Environment.GetEnvironmentVariable("DB_HOST");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = $"Server={hostname};Database=UrlDb;User Id=SA;Password={password};Trusted_Connection=False;TrustServerCertificate=True";
        options.UseSqlServer(connectionString);
    }
}
