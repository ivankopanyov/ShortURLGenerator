namespace ShortURLGenerator.QRCreator.API.Services.QRCodeCreation;

public interface IQRCodeCreationService
{
    byte[] GenerateJpeg(string uri, int size);
}

