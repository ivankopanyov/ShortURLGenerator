namespace ShortURLGenerator.GrpcHelper.Services.UrlServiceClientFactory;

/// <summary>
/// Factory for creating a <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" /> object.
/// </summary>
public interface IUrlServiceClientFactory
{
    /// <summary>Creates a <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" /> for the specified channel.</summary>
    /// <param name="channel">Channel.</param>
    /// <returns>A new instance of <see cref="T:ShortURLGenerator.Grpc.Services.UrlService.UrlServiceClient" />.</returns>
    UrlService.UrlServiceClient New(ChannelBase channel);
}

