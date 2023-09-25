namespace ShortURLGenerator.GrpcHelper.Services.UrlServiceClientFactory;

/// <summary>
/// Сlass that describes a factory for creating a <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" /> object.
/// Implements the interface <see cref="T:ShortURLGenerator.GrpcHelper.Services.UrlServiceClientFactory.IUrlServiceClientFactory" />.
/// </summary>
public class UrlServiceClientFactory : IUrlServiceClientFactory
{
    /// <summary>Creates a <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" /> for the specified channel.</summary>
    /// <param name="channel">Channel.</param>
    /// <returns>A new instance of <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" />.</returns>
    public UrlService.UrlServiceClient New(ChannelBase channel) =>
        new UrlService.UrlServiceClient(channel);
}

