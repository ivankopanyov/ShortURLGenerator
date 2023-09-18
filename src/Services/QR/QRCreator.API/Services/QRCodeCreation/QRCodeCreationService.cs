namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>
/// Class that describes a service for creating QR codes.
/// Uses the QRCoder library.
/// Implements the IQRCodeCreationService interface.
/// </summary>
public class QRCodeCreationService : IQRCodeCreationService
{
    /// <summary>
    /// Default QR code size.
    /// Set if the size is not specified or is incorrect.
    /// </summary>
    private const int DEFAULT_SIZE_PIXELS = 1;

    /// <summary>Log service.</summary>
	private readonly ILogger _logger;

    /// <summary>The size of the side of the image with the QR code.</summary>
    private readonly int _sizePixels;

    /// <summary>Initialization of the service object for creating QR codes.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public QRCodeCreationService(ILogger<QRCodeCreationService> logger, IConfiguration? configuration = null)
	{
		_logger = logger;

        var qRCodeCreationServiceConfiguration = new QRCodeCreationServiceConfiguration();
        OnConfiguring(qRCodeCreationServiceConfiguration, configuration);

        _sizePixels = Math.Max(DEFAULT_SIZE_PIXELS, qRCodeCreationServiceConfiguration.SizePixels);
	}

    /// <summary>Method for generating a QR code in JPEG format</summary>
    /// <param name="uri">Source URI.</param>
    /// <returns>QR code data.</returns>
    public byte[] GenerateJpeg(string uri)
    {
        _logger.LogInformation($"Create QR code: Start. URI: {uri}.");

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

        _logger.LogInformation($"Create QR code: Successfully. URI: {uri}.");

        return qrCode.GetGraphic(_sizePixels);
    }

    /// <summary>Virtual method for configuring a service for generating QR codes.</summary>
    /// <param name="serviceConfiguration">Service configuration object.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected virtual void OnConfiguring(QRCodeCreationServiceConfiguration serviceConfiguration, IConfiguration? appConfiguration)
    {
        if (appConfiguration != null)
        {
            var sizePixels = appConfiguration
                .GetSection("QRCode")
                .GetValue<int>("SizePixels");

            serviceConfiguration.SizePixels = Math.Max(DEFAULT_SIZE_PIXELS, sizePixels);
        }
        else
            serviceConfiguration.SizePixels = DEFAULT_SIZE_PIXELS;
    }
}

