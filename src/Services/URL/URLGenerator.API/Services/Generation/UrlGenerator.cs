namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

public class UrlGenerator : UrlGeneratorBase
{
	public UrlGenerator(ILogger<UrlGenerator> logger, IConfiguration configuration)
		: base(logger, configuration) { }

    protected override void OnConfiguring(UrlGeneratorConfiguration configuration)
    {
        configuration.UrlLength = AppConfiguration.GetSection("URL").GetValue<int>("Length");
        configuration.SourceSymbols = AppConfiguration.GetSection("URL").GetValue<string>("SourceSymbols")!;
    }
}

