namespace ShortURLGenerator.GrpcHelper.Services.GrpcChannelFactory;

/// <summary>
/// Сlass that describes a factory for creating a <see cref="T:Grpc.Net.Client.GrpcChannel" /> object.
/// Implements the interface <see cref="T:ShortURLGenerator.GrpcHelper.Services.GrpcChannelFactory.IGrpcChannelFactory" />.
/// </summary>
public class GrpcChannelFactory : IGrpcChannelFactory
{
    /// <summary>Creates a <see cref="T:Grpc.Net.Client.GrpcChannel" /> for the specified address.</summary>
    /// <param name="address">The address the channel will use.</param>
    /// <returns>A new instance of <see cref="T:Grpc.Net.Client.GrpcChannel" />.</returns>
    public GrpcChannel ForAddress(string address) => GrpcChannel.ForAddress(address);
}

