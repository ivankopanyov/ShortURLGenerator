namespace ShortURLGenerator.URLGenerator.API.Extensions;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services) =>
        services.AddStackExchangeRedisCache(options =>
        {
            var host = Environment.GetEnvironmentVariable("CACHE_HOST");
            var port = Environment.GetEnvironmentVariable("CACHE_PORT");
            options.Configuration = $"{host}:{port}";
            options.InstanceName = "url";
        });
}

