namespace ShortURLGenerator.StringGenerator;

/// <summary>
/// Class that describes a random string generator.
/// Implements the IGeneratable interface.
/// </summary>
public class StringGenerator : IGeneratable
{
    /// <summary>The value of the string with default source characters.</summary>
    private const string DEFAULT_SOURCE_SYMBOLS = "0123456789";

    /// <summary>Default length of generated string.</summary>
    private const int DEFAULT_STRING_LENGTH = 10;

    /// <summary>The length of the generated string.</summary>
    private readonly int _stringLength;

    /// <summary>String with the character set to generate.</summary>
    private readonly string _sourceSymbols;

    /// <summary>Initialization of the random string generator object.</summary>
    /// <param name="configuration">Application configuration.</param>
    public StringGenerator(IConfiguration configuration)
    {
        var stringGeneratorConfiguration = new StringGeneratorConfiguration();
        OnConfiguring(stringGeneratorConfiguration, configuration);

        _sourceSymbols = !string.IsNullOrEmpty(stringGeneratorConfiguration.SourceSymbols)
            ? stringGeneratorConfiguration.SourceSymbols
            : DEFAULT_SOURCE_SYMBOLS;

        _stringLength = stringGeneratorConfiguration.StringLength > 0
            ? stringGeneratorConfiguration.StringLength
            : DEFAULT_STRING_LENGTH;
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
    /// <param name="stringGeneratorConfiguration">Random string generator configuration object.</param>
    /// <param name="configuration">Application configuration.</param>Application configuration.
    protected virtual void OnConfiguring(StringGeneratorConfiguration stringGeneratorConfiguration,
        IConfiguration configuration)
    {
        stringGeneratorConfiguration.StringLength = configuration
            .GetSection("StringGenerator")
            .GetValue<int>("Length");

        stringGeneratorConfiguration.SourceSymbols = configuration
            .GetSection("StringGenerator")
            .GetValue<string>("SourceSymbols")!;
    }
}
