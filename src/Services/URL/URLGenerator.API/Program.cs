var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
    options.Listen(IPAddress.Any, 80, ListenOptions => ListenOptions.Protocols = HttpProtocols.Http2));

builder.Services.AddGrpc();
builder.Services.AddDbContext<UrlContext>();
builder.Services.AddRedis();

builder.Services
    .AddScoped<IUrlRepository, UrlRepository>()
    .AddScoped<IGeneratable, UrlGenerator>();

var app = builder.Build();

app.MapGrpcService<UrlService>();

using (var client = new UrlContext())
{
    client.Database.Migrate();
}

app.Run();

