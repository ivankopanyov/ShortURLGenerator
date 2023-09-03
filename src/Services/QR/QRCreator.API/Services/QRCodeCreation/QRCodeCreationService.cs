namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>Class that describes a service for creating QR codes. Uses the BarCode library.</summary>
public class QRCodeCreationService : IQRCodeCreationService
{
    /// <summary>Log service.</summary>
	private readonly ILogger _logger;

    /// <summary>Initialization of the service object for creating QR codes.</summary>
    /// <param name="logger">Log service.</param>
	public QRCodeCreationService(ILogger<QRCodeCreationService> logger)
	{
		_logger = logger;
	}

    /// <summary>Method for generating a QR code in JPEG format</summary>
    /// <param name="uri">Source URI.</param>
    /// <param name="size">The size of the side of the image with the QR code.</param>
    /// <returns>QR code data.</returns>
    public byte[] GenerateJpeg(string uri, int size)
    {
        if (size < 100)
            size = 100;

        _logger.LogInformation($"Create QR code: start.\n\tURI: {uri}\n\tSize: {size}");
        GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(uri, size, QRCodeWriter.QrErrorCorrectionLevel.Medium);
        _logger.LogInformation($"Create QR code: succesful.\n\tURI: {uri}\n\tSize: {size}");
        return barcode.ToJpegBinaryData();
    }
}

