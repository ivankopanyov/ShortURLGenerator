namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

/// <summary>
/// Class that describes a random string generator.
/// Inherited from class UrlGeneratorBase.
/// Implements the IGeneratable interface.
/// </summary>
public class UrlGenerator : UrlGeneratorBase
{
    /// <summary>Initialization of the random string generator object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public UrlGenerator(ILogger<UrlGenerator> logger, IConfiguration configuration)
		: base(logger, configuration) { }

    /// <summary>Method overriding for configuring the random string generator.</summary>
    /// <param name="configuration">Random string generator configuration object.</param>
    protected override void OnConfiguring(UrlGeneratorConfiguration configuration)
    {
        configuration.UrlLength = AppConfiguration.GetSection("URL").GetValue<int>("Length");
        configuration.SourceSymbols = AppConfiguration.GetSection("URL").GetValue<string>("SourceSymbols")!;
    }
}

