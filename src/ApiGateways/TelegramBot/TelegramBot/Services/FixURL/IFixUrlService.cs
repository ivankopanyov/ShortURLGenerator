namespace ShortURLGenerator.TelegramBot.Services.FixURL;

/// <summary>Service for fixing URL.</summary>
public interface IFixUrlService
{
    /// <summary>URL fix method.</summary>
    /// <param name="url">Source URL.</param>
    /// <returns>Corrected URL.</returns>
    public string FixUrl(string url);
}
