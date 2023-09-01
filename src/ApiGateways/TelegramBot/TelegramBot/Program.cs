AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddSingleton<ITelegramBot, TelegramBot>()
    .AddScoped<IUpdateHandler, UpdateHandler>()
    .AddScoped<IIdentityService, IdentityMockService>()
    .AddScoped<IUrlService, UrlMockService>();

var app = builder.Build();

app.MapControllers();

app.Run();
