namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

public abstract class UrlGeneratorBase : IGeneratable
{
    private readonly ILogger _logger;

    private readonly int _urlLength;

    private readonly string _sourceSymbols;

    protected IConfiguration AppConfiguration { get; private init; }

    public UrlGeneratorBase(ILogger<UrlGeneratorBase> logger, IConfiguration configuration)
	{
        _logger = logger;
        AppConfiguration = configuration;

        var urlGeneratorConfiguration = new UrlGeneratorConfiguration();
        OnConfiguring(urlGeneratorConfiguration);

        _sourceSymbols = !string.IsNullOrEmpty(urlGeneratorConfiguration.SourceSymbols)
            ? urlGeneratorConfiguration.SourceSymbols
            : throw new ArgumentNullException(nameof(urlGeneratorConfiguration.SourceSymbols));

        _urlLength = Math.Max(0, urlGeneratorConfiguration.UrlLength);
    }

    public string GenerateString()
    {
        _logger.LogInformation("Generate URL: start.");

        var result = string.Empty;
        for (int i = 0; i < _urlLength; i++)
            result += _sourceSymbols[Random.Shared.Next(_sourceSymbols.Length)];

        _logger.LogInformation($"Generate URL: succesful.\n\tURL: {result}");

        return result;
    }

    protected abstract void OnConfiguring(UrlGeneratorConfiguration configuration);
}

