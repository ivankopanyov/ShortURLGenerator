namespace ShortURLGenerator.Identity.API.Services.VerificationCodeGenerator;

/// <summary>
/// Class that describes verification code generation service.
/// Inherited from the StringGenerator.StringGenerator class from the StringGenerator assembly.
/// Implements the IVerificationCodeGenerationService interface.
/// </summary>
public class VerificationCodeGenerationService : StringGenerator.StringGenerator, IVerificationCodeGenerationService
{
    /// <summary>Initializing a service object.</summary>
    /// <param name="configuration">Application configuration.</param>
    public VerificationCodeGenerationService(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// Overriding the service configuration method.
    /// To set the verification code length value, use the value of the "Length" field
    /// from the "VerificationCode" section of the application configuration.
    /// To set the verification code source symbols value, use the value of the "SourceSymbols" field
    /// from the "VerificationCode" section of the application configuration.
    /// </summary>
    /// <param name="stringGeneratorConfiguration">String generator configuration.</param>
    /// <param name="configuration">Application configuration.</param>
    protected override void OnConfiguring(StringGeneratorConfiguration stringGeneratorConfiguration,
        IConfiguration configuration)
    {
        stringGeneratorConfiguration.StringLength = configuration
            .GetSection("VerificationCode")
            .GetValue<int>("Length");

        stringGeneratorConfiguration.SourceSymbols = configuration
            .GetSection("VerificationCode")
            .GetValue<string>("SourceSymbols")!;
    }
}
