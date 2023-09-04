namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

public class UrlGenerator : IGeneratable
{
    private readonly ILogger _logger;

    private readonly int _urlLength;

    private readonly string _sourceSymbols;

	public UrlGenerator(ILogger<UrlGenerator> logger, IConfiguration configuration)
	{
        _logger = logger;
        _urlLength = configuration.GetSection("URL").GetValue<int>("Length");
        _sourceSymbols = configuration.GetSection("URL").GetValue<string>("Source")!;
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
}

