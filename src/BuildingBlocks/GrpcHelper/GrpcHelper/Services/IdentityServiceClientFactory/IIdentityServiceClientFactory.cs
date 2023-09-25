namespace ShortURLGenerator.GrpcHelper.Services.IdentityServiceClientFactory;

/// <summary>
/// Factory for creating a <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" /> object.
/// </summary>
public interface IIdentityServiceClientFactory
{
    /// <summary>Creates a <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" /> for the specified channel.</summary>
    /// <param name="channel">Channel.</param>
    /// <returns>A new instance of <see cref="T:ShortURLGenerator.Grpc.Services.IdentityService.IdentityServiceClient" />.</returns>
    IdentityService.IdentityServiceClient New(ChannelBase channel);
}

 