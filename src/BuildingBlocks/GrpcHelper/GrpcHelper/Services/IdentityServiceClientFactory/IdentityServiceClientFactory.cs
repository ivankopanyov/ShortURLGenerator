namespace ShortURLGenerator.GrpcHelper.Services.IdentityServiceClientFactory;

/// <summary>
/// Сlass that describes a factory for creating a <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" /> object.
/// Implements the interface <see cref="T:ShortURLGenerator.GrpcHelper.Services.IdentityServiceClientFactory.IIdentityServiceClientFactory" />.
/// </summary>
public class IdentityServiceClientFactory : IIdentityServiceClientFactory
{
    /// <summary>Creates a <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" /> for the specified channel.</summary>
    /// <param name="channel">Channel.</param>
    /// <returns>A new instance of <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" />.</returns>
    public IdentityService.IdentityServiceClient New(ChannelBase channel) =>
        new IdentityService.IdentityServiceClient(channel);
}

