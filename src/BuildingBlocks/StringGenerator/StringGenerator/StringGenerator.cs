namespace ShortURLGenerator.StringGenerator;

/// <summary>
/// Class that describes a random string generator.
/// Implements the IGeneratable interface.
/// </summary>
public class StringGenerator : IGeneratable
{
    /// <summary>The value of the string with default source characters.</summary>
    public const string DEFAULT_SOURCE_SYMBOLS = "0123456789";

    /// <summary>Default length of generated string.</summary>
    public const int DEFAULT_STRING_LENGTH = 1;

    /// <summary>The length of the generated string.</summary>
    private readonly int _stringLength;

    /// <summary>String with the character set to generate.</summary>
    private readonly string _sourceSymbols;

    /// <summary>Initialization of the random string generator object.</summary>
    /// <param name="configuration">Random string generator configuration object.</param>
    public StringGenerator(StringGeneratorConfiguration configuration)
    {
        if (configuration is null)
            configuration = new StringGeneratorConfiguration();

        OnConfiguring(configuration, null);

        _stringLength = Math.Max(DEFAULT_STRING_LENGTH, configuration.StringLength);
        _sourceSymbols = string.IsNullOrEmpty(configuration.SourceSymbols)
                ? DEFAULT_SOURCE_SYMBOLS
                : configuration.SourceSymbols;
    }

    /// <summary>Initialization of the random string generator object.</summary>
    /// <param name="configuration">Application configuration.</param>
    public StringGenerator(IConfiguration? configuration = null)
    {
        var stringGeneratorConfiguration = new StringGeneratorConfiguration();
        OnConfiguring(stringGeneratorConfiguration, configuration);

        _stringLength = Math.Max(DEFAULT_STRING_LENGTH, stringGeneratorConfiguration.StringLength);
        _sourceSymbols = string.IsNullOrEmpty(stringGeneratorConfiguration.SourceSymbols)
                ? DEFAULT_SOURCE_SYMBOLS
                : stringGeneratorConfiguration.SourceSymbols;
    }

    /// <summary>Method for generating a random string.</summary>
    /// <returns>Random string.</returns>
    public string GenerateString()
    {
        var stringBuilder = new StringBuilder(capacity: _stringLength, maxCapacity: _stringLength);

        for (int i = 0; i < _stringLength; i++)
            stringBuilder.Append(_sourceSymbols[Random.Shared.Next(_sourceSymbols.Length)]);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Virtual method for configuring the random string generator.
    /// By default, sets the values ​​from the "StringGenerator" section of the application configuration.
    /// The length of the generated string is set from the parameter with the "Length" key.
    /// The value of the source string is set from the parameter with the "SourceString" key.
    /// </summary>
    /// <param name="generatorConfiguration">Random string generator configuration object.</param>
    /// <param name="appConfiguration">Application configuration.</param>Application configuration.
    protected virtual void OnConfiguring(StringGeneratorConfiguration generatorConfiguration,
        IConfiguration? appConfiguration)
    {

        if (appConfiguration != null)
        {
            var length = appConfiguration
                .GetSection("StringGenerator")
                .GetValue<int>("Length");

            var sourceSymbols = appConfiguration
                .GetSection("StringGenerator")
                .GetValue<string>("SourceSymbols");

            generatorConfiguration.StringLength = Math.Max(DEFAULT_STRING_LENGTH, length);

            generatorConfiguration.SourceSymbols = string.IsNullOrEmpty(sourceSymbols)
                ? DEFAULT_SOURCE_SYMBOLS
                : sourceSymbols;
        }
        else
        {
            generatorConfiguration.StringLength = DEFAULT_STRING_LENGTH;
            generatorConfiguration.SourceSymbols = DEFAULT_SOURCE_SYMBOLS;
        }
    }
}
