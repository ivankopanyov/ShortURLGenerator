var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
    options.Listen(IPAddress.Any, 80, ListenOptions => ListenOptions.Protocols = HttpProtocols.Http2));

builder.Services.AddGrpc();
builder.Services.AddRedis();

builder.Services
    .AddScoped<IVerificationCodeGenerationService, VerificationCodeGenerationService>()
    .AddScoped<IRefreshTokenGenerationService, RefreshTokenGenerationService>()
    .AddScoped<IAccessTokenGenerationService, JwtGenerationService>()
    .AddScoped<IVerificationCodeRepository, VerificationCodeRepository>()
    .AddScoped<IConnectionRepository, ConnectionRepository>();

var app = builder.Build();

app.MapGrpcService<IdentityService>();

app.Run();
