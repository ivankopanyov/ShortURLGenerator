namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

/// <summary>
/// <summary>Class that describes a request to create a sign in.</summary></summary>
public class SingInDto
{
    /// <summary>Verification code.</summary>
    public string VerificationCode { get; set; }

    /// <summary>Connection info.</summary>
    public Grpc.Services.ConnectionInfo ConnectionInfo { get; set; }

    /// <summary>Overriding the method for converting an object to a string type.</summary>
    /// <returns>Object converted to a string type.</returns>
    public override string ToString() =>
        "Sing in: { Verification code: " + VerificationCode +
        ", " + ConnectionInfo?.LogInfo() + " }";
}
