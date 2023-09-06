namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;

/// <summary>Class that describes the configuration of a connection command.</summary>
public class ConnectionsCommandConfiguration
{
    /// <summary>The number of active connections on the page.</summary>
	public int PageSize { get; set; }
}

