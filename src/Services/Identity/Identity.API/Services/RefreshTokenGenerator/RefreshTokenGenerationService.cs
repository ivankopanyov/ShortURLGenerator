namespace ShortURLGenerator.Identity.API.Services.RefreshTokenGenerator;

/// <summary>
/// Class that describes refresh token generation service.
/// Inherited from the StringGenerator.StringGenerator class from the StringGenerator assembly.
/// Implements the IRefreshTokenGenerationService interface.
/// </summary>
public class RefreshTokenGenerationService : StringGenerator.StringGenerator, IRefreshTokenGenerationService
{
    /// <summary>Initializing a service object.</summary>
    /// <param name="configuration">Application configuration.</param>
    public RefreshTokenGenerationService(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// Overriding the service configuration method.
    /// To set the refresh token length value, use the value of the "Length" field
    /// from the "RefreshToken" section of the application configuration.
    /// To set the refresh token source symbols value, use the value of the "SourceSymbols" field
    /// from the "RefreshToken" section of the application configuration.
    /// </summary>
    /// <param name="generatorConfiguration">String generator configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected override void OnConfiguring(StringGeneratorConfiguration generatorConfiguration,
        IConfiguration? appConfiguration)
    {
        if (appConfiguration is null)
            return;

        generatorConfiguration.StringLength = appConfiguration
            .GetSection("RefreshToken")
            .GetValue<int>("Length");

        generatorConfiguration.SourceSymbols = appConfiguration
            .GetSection("RefreshToken")
            .GetValue<string>("SourceSymbols")!;
    }
}
