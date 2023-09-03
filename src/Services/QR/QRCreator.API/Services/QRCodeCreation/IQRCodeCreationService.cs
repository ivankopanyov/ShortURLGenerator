namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>Service for creating QR codes.</summary>
public interface IQRCodeCreationService
{
    /// <summary>Method for generating a QR code in JPEG format</summary>
    /// <param name="uri">Source URI.</param>
    /// <param name="size">The size of the side of the image with the QR code.</param>
    /// <returns>QR code data.</returns>
    byte[] GenerateJpeg(string uri, int size);
}

