namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

/// <summary>
/// Class that describes a random string generator.
/// Implements the IGeneratable interface.
/// </summary>
public class UrlGenerator : IGeneratable
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Application configuration.</summary>
    private readonly IConfiguration _appConfiguration;

    /// <summary>The length of the generated string.</summary>
    private readonly int _urlLength;

    /// <summary>String with the character set to generate.</summary>
    private readonly string _sourceSymbols;

    /// <summary>Initialization of the random string generator object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <exception cref="ArgumentNullException">
    /// Exception is thrown if the application configuration does not contain source symbols or if source simbols is an empty string.
    /// </exception>
    public UrlGenerator(ILogger<UrlGenerator> logger, IConfiguration configuration)
	{
        _logger = logger;
        _appConfiguration = configuration;

        var urlGeneratorConfiguration = new UrlGeneratorConfiguration();
        OnConfiguring(urlGeneratorConfiguration);

        _sourceSymbols = !string.IsNullOrEmpty(urlGeneratorConfiguration.SourceSymbols)
            ? urlGeneratorConfiguration.SourceSymbols
            : throw new ArgumentNullException(nameof(urlGeneratorConfiguration.SourceSymbols));

        _urlLength = Math.Max(0, urlGeneratorConfiguration.UrlLength);
    }

    /// <summary>Method for generating a random string.</summary>
    /// <returns>Random string.</returns>
    public string GenerateString()
    {
        _logger.LogStart("Generate URL");

        var result = string.Empty;
        for (int i = 0; i < _urlLength; i++)
            result += _sourceSymbols[Random.Shared.Next(_sourceSymbols.Length)];

        _logger.LogObject("Generate URL", result);
        _logger.LogSuccessfully("Generate URL");

        return result;
    }

    /// <summary>Virtual method for configuring the random string generator.</summary>
    /// <param name="configuration">Random string generator configuration object.</param>
    protected virtual void OnConfiguring(UrlGeneratorConfiguration configuration)
    {
        configuration.UrlLength = _appConfiguration.GetSection("URL").GetValue<int>("Length");
        configuration.SourceSymbols = _appConfiguration.GetSection("URL").GetValue<string>("SourceSymbols")!;
    }
}

