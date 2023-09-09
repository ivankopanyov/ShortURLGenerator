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
            var host = Environment.GetEnvironmentVariable("REDIS_HOST");
            var port = Environment.GetEnvironmentVariable("REDIS_PORT");
            options.Configuration = $"{host}:{port}";
            options.InstanceName = "identity";
        });
}

