namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

public class RefreshTokenDto
{
    public Token Token { get; set; }

    public Grpc.Services.ConnectionInfo ConnectionInfo { get; set; }
}
