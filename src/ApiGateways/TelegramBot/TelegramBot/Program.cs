AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddSingleton<IEventBus, EventBusRabbitMQ>()
    .AddSingleton<IFixUrlService, FixUrlService>()
    .AddSingleton<ITelegramBot, TelegramBot>();

builder.Services
    .AddScoped<IUpdateHandler, UpdateHandler>()
    .AddScoped<IGrpcChannelFactory, GrpcChannelFactory>()
    .AddScoped<IUrlServiceClientFactory, UrlServiceClientFactory>()
    .AddScoped<IIdentityServiceClientFactory, IdentityServiceClientFactory>()
    .AddScoped<IConnectionService, ShortURLGenerator.GrpcHelper.Services.Identity.IdentityService>()
    .AddScoped<IUrlGenerator, ShortURLGenerator.GrpcHelper.Services.URL.UrlService>();

builder.Services
    .AddHostedService<UriGeneratedIntegrationEventHandler>()
    .AddHostedService<QRCodeCreatedIntegrationEventHandler>(); 

var app = builder.Build();

app.MapControllers();

app.Run();
