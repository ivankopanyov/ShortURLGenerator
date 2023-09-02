namespace ShortURLGenerator.EventBus;

/// <summary>Class that describes the sender of integration events in RabbitMQ.</summary>
public abstract class EventBusRabbitMQBase : IEventBus
{
    /// <summary>Factory for creating connections to RabbitMQ.</summary>
    private readonly ConnectionFactory _connectionFactory;

    /// <summary>Initializing the EventBus object.</summary>
    public EventBusRabbitMQBase()
    {
        _connectionFactory = new ConnectionFactory();
        OnConfigureConnection(_connectionFactory);
    }

    /// <summary>Method for publishing an RabbitMQ.</summary>
    /// <param name="event">Integration event.</param>
    public void Publish(IntegrationEventBase @event)
    {
        string queueName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event);

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

    /// <summary>Abstract method for configuring a connection to RabbitMQ.</summary>
    /// <param name="connectionFactory">Factory for creating connections to RabbitMQ.</param>
    protected abstract void OnConfigureConnection(ConnectionFactory connectionFactory);
}

