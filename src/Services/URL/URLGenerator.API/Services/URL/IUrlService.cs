namespace ShortURLGenerator.URLGenerator.API.Services.URL;

public interface IUrlService
{
    Task<UrlResponseDto> Generate(SourceUriDto request, Grpc.Core.ServerCallContext context);
}

