namespace ShortURLGenerator.TelegramBot.Services.Url;

public class UrlMockService : IUrlService
{
    public async Task<string> GenerateUrlAsync(string sourceUri)
    {
        if (sourceUri is null)
            throw new ArgumentNullException(nameof(sourceUri));

        return await Task.Run(() => "some_url");
    }
}

