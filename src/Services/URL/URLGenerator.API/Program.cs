var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
    client.Database.EnsureDeleted();
    client.Database.EnsureCreated();
}

app.Run();

