namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>
/// Class that describes a service for creating QR codes.
/// Uses the BarCode library.
/// Implements the IQRCodeCreationService interface.
/// </summary>
public class QRCodeCreationService : IQRCodeCreationService
{
    /// <summary>Log service.</summary>
	private readonly ILogger _logger;

    /// <summary>Application configuration.</summary>
    private readonly IConfiguration _appConfiguration;

    /// <summary>The size of the side of the image with the QR code.</summary>
    private readonly int _sizePixels;

    /// <summary>Initialization of the service object for creating QR codes.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public QRCodeCreationService(ILogger<QRCodeCreationService> logger, IConfiguration configuration)
	{
		_logger = logger;
        _appConfiguration = configuration;

        var qRCodeCreationServiceConfiguration = new QRCodeCreationServiceConfiguration();
        OnConfiguring(qRCodeCreationServiceConfiguration);
        _sizePixels = Math.Max(50, qRCodeCreationServiceConfiguration.SizePixels);

        _appConfiguration = configuration;
	}

    /// <summary>Method for generating a QR code in JPEG format</summary>
    /// <param name="uri">Source URI.</param>
    /// <returns>QR code data.</returns>
    public byte[] GenerateJpeg(string uri)
    {
        _logger.LogStart("Create QR code", uri);

        GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(uri, _sizePixels, QRCodeWriter.QrErrorCorrectionLevel.Medium);

        _logger.LogObject("Create QR code", barcode);
        _logger.LogSuccessfully("Create QR code", uri);

        return barcode.ToJpegBinaryData();
    }

    /// <summary>Virtual method for configuring a service for generating QR codes.</summary>
    /// <param name="configuration">Service configuration object.</param>
    protected virtual void OnConfiguring(QRCodeCreationServiceConfiguration configuration)
    {
        configuration.SizePixels = _appConfiguration.GetSection("QRCode").GetValue<int>("SizePixels");
    }
}

