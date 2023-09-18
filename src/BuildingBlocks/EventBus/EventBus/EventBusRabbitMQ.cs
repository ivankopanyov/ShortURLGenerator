namespace ShortURLGenerator.EventBus;

/// <summary>Class that describes the sender of integration events in RabbitMQ.</summary>
public class EventBusRabbitMQ : IEventBus
{
    /// <summary>Factory for creating connections to RabbitMQ.</summary>
    private readonly ConnectionFactory _connectionFactory;

    /// <summary>Initializing the EventBus object.</summary>
    public EventBusRabbitMQ()
    {
        _connectionFactory = new ConnectionFactory();
        OnConfigureConnection(_connectionFactory);
    }

    /// <summary>Method for publishing an RabbitMQ.</summary>
    /// <typeparam name="T">Integration event type.</typeparam>
    /// <param name="event">Integration event.</param>
    /// <exception cref="InvalidOperationException">Failed to connect to the broker.</exception>
    public void Publish<T>(T @event) where T : IntegrationEventBase
    {
        string queueName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event);

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                            routingKey: queueName,
                            basicProperties: null,
                            body: body);
        }
        catch (BrokerUnreachableException ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    /// <summary>Virtual method for configuring a connection to RabbitMQ.</summary>
    /// <param name="connectionFactory">Factory for creating connections to RabbitMQ.</param>
    protected virtual void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

