namespace ShortURLGenerator.TelegramBot.Services.Url;

public interface IUrlService
{
    Task<string> GenerateUrlAsync(string sourceUri);
}

