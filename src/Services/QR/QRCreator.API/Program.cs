var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IQRCodeCreationService, QRCodeCreationService>()
    .AddSingleton<IEventBus, EventBusRabbitMQ>();

builder.Services
    .AddHostedService<UriSentIntegrationEventHandler>();

var app = builder.Build();

app.Run();

