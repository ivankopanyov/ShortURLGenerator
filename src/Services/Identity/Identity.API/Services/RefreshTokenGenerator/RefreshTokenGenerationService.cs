namespace ShortURLGenerator.Identity.API.Services.RefreshTokenGenerator;

public class RefreshTokenGenerationService : StringGenerator.StringGenerator, IRefreshTokenGenerationService
{
    public RefreshTokenGenerationService(IConfiguration configuration) : base(configuration) { }

    protected override void OnConfiguring(StringGeneratorConfiguration stringGeneratorConfiguration,
        IConfiguration configuration)
    {
        stringGeneratorConfiguration.StringLength = configuration
            .GetSection("RefreshToken")
            .GetValue<int>("Length");

        stringGeneratorConfiguration.SourceSymbols = configuration
            .GetSection("RefreshToken")
            .GetValue<string>("SourceSymbols")!;
    }
}
