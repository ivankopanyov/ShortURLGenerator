namespace ShortURLGenerator.Identity.API.Extensions;

/// <summary>Static extension class.</summary>
public static class Extensions
{
    /// <summary>Static method for adding Redis to dependency injection.</summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddRedis(this IServiceCollection services) =>
        services.AddStackExchangeRedisCache(options =>
        {
            var host = Environment.GetEnvironmentVariable("CACHE_HOST");
            var port = Environment.GetEnvironmentVariable("CACHE_PORT");
            options.Configuration = $"{host}:{port}";
            options.InstanceName = "url";
        });
}

