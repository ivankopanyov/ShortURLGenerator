namespace ShortURLGenerator.EventBus.Extansions;

/// <summary>Extensions of the ConnectionFactory class.</summary>
public static class ConnectionFactoryExtansions
{
    /// <summary>Multiple attempts to connect to the broker with a delay.</summary>
    /// <param name="connectionFactory">ConnectionFactory object.</param>
    /// <param name="tryCount">The number of connection attempts.</param>
    /// <param name="delay">Delay between connection attempts.</param>
    /// <returns>Connection to a broker.</returns>
    /// <exception cref="BrokerUnreachableException">Failed to connect to the broker.</exception>
    public static IConnection TryCreateConnection(this ConnectionFactory connectionFactory, int tryCount, TimeSpan delay)
    {
        if (tryCount < 1)
            tryCount = 1;

        IConnection? connection = null;

        for (int i = 0; i < tryCount; i++)
        {
            try
            {
                connection = connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                if (i == tryCount - 1)
                    throw new BrokerUnreachableException(ex);

                Thread.Sleep(delay);
            }
        }

        return connection!;
    }
}

