namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>
/// Class that describes a service for creating QR codes.
/// Inherited from QRCodeCreationService.
/// </summary>
public class QRCodeCreationService : QRCodeCreationServiceBase
{
    /// <summary>Initialization of the service object for creating QR codes.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public QRCodeCreationService(ILogger<QRCodeCreationService> logger, IConfiguration configuration)
		: base(logger, configuration) { }

    /// <summary>Overriding the method of configuring the service for generating QR codes.</summary>
    /// <param name="configuration">Service configuration object.</param>
    protected override void OnConfiguring(QRCodeCreationServiceConfiguration configuration)
    {
        configuration.SizePixels = AppConfiguration.GetSection("QRCode").GetValue<int>("SizePixels");
    }
}

