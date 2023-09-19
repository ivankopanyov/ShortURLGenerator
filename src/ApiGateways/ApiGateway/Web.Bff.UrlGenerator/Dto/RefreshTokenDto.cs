namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

/// <summary>Class that describes a request to create a refresh token.</summary>
public class RefreshTokenDto
{
    /// <summary>Token.</summary>
    public Token Token { get; set; }

    /// <summary>Connection info.</summary>
    public Grpc.Services.ConnectionInfo ConnectionInfo { get; set; }

    /// <summary>Overriding the method for converting an object to a string type.</summary>
    /// <returns>Object converted to a string type.</returns>
    public override string ToString() =>
        "Refresh token DTO: { " + Token.LogInfo() +
        ", " + ConnectionInfo.LogInfo() + " }";
}
