var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
    options.Listen(IPAddress.Any, 80, ListenOptions => ListenOptions.Protocols = HttpProtocols.Http2));

builder.Services.AddGrpc();
builder.Services.AddRedis();

builder.Services
    .AddScoped<IIdentityService, IdentityService>();

var app = builder.Build();

app.MapGrpcService<IdentityService>();

app.Run();
