namespace ShortURLGenerator.EventBus.Handling;

/// <summary>Base class that describes the integration event handler. Inherits from BackgroundService.</summary>
/// <typeparam name="T">Integration event type.</typeparam>
public abstract class IntegrationEventHandlerBase<T> : BackgroundService where T : IntegrationEventBase
{
    /// <summary>The name of the integration event type is used as the queue name.</summary>
    private string _queueName = typeof(T).Name;

    /// <summary>Connecting to a message broker.</summary>
    private IConnection _connection;

    /// <summary>Channel.</summary>
    private IModel _channel;

    /// <summary>Initialization of the base integration event handler.</summary>
    public IntegrationEventHandlerBase()
    {
        var factory = new ConnectionFactory();
        OnConfiguringConnection(factory);
        _connection = factory.TryCreateConnection(10, TimeSpan.FromSeconds(5));
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    /// <summary>Redefining the base method for executing an integration event.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    protected override sealed Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, eventArgs) =>
        {
            var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var @event = JsonSerializer.Deserialize<T>(content);
            await HandleAsync(@event);
            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(_queueName, false, consumer);
        return Task.CompletedTask;
    }

    /// <summary>Abstract method for configuring a connection to a message broker.</summary>
    /// <param name="connectionFactory">Factory for creating connections to the message broker.</param>
    protected abstract void OnConfiguringConnection(ConnectionFactory connectionFactory);

    /// <summary>Abstract event handling method.</summary>
    /// <param name="event">Event.</param>
    protected abstract Task HandleAsync(T? @event);

    /// <summary>Overriding the method for closing the connection to the message broker.</summary>
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}

