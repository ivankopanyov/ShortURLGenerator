namespace ShortURLGenerator.Web.Bff.UrlGenerator.Services.EventBus;

/// <summary>RabbitMQ service.</summary>
public class EventBusRabbitMQ : EventBusRabbitMQBase
{
    /// <summary>Overriding the connection method to RabbitMQ.</summary>
    /// <param name="connectionFactory">Connection factory</param>
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

