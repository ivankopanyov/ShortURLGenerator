namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

/// <summary>Service for creating QR codes.</summary>
public interface IQRCodeCreationService
{
    /// <summary>Method for generating a QR code in JPEG format</summary>
    /// <param name="uri">Source URI.</param>
    /// <returns>QR code data.</returns>
    byte[] GenerateJpeg(string uri);
}

