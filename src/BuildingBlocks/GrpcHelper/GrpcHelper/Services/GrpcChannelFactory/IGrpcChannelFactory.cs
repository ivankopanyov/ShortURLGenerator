namespace ShortURLGenerator.GrpcHelper.Services.GrpcChannelFactory;

/// <summary>Factory for creating a <see cref="T:Grpc.Net.Client.GrpcChannel" /> object.</summary>
public interface IGrpcChannelFactory
{
    /// <summary>Creates a <see cref="T:Grpc.Net.Client.GrpcChannel" /> for the specified address.</summary>
    /// <param name="address">The address the channel will use.</param>
    /// <returns>A new instance of <see cref="T:Grpc.Net.Client.GrpcChannel" />.</returns>
    GrpcChannel ForAddress(string address);
}

