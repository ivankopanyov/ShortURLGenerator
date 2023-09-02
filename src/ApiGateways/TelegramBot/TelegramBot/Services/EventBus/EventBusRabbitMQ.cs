namespace ShortURLGenerator.TelegramBot.Services.EventBus;

public class EventBusRabbitMQ : EventBusRabbitMQBase
{
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

