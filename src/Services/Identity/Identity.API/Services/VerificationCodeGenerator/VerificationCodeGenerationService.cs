namespace ShortURLGenerator.Identity.API.Services.VerificationCodeGenerator;

public class VerificationCodeGenerationService : StringGenerator.StringGenerator, IVerificationCodeGenerationService
{
    public VerificationCodeGenerationService(IConfiguration configuration) : base(configuration) { }

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
