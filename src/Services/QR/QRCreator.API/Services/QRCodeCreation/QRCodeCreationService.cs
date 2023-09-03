using IronBarCode;

namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

public class QRCodeCreationService : IQRCodeCreationService
{
	private readonly ILogger _logger;

	public QRCodeCreationService(ILogger<QRCodeCreationService> logger)
	{
		_logger = logger;
	}

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

