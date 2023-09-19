namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

public class SingInDto
{
    public string VerificationCode { get; set; }

    public Grpc.Services.ConnectionInfo ConnectionInfo { get; set; }

    public override string ToString() =>
        "Sing in: { Verification code: " + VerificationCode +
        ", " + ConnectionInfo?.LogInfo() + " }";
}
