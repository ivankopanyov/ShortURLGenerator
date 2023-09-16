namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

public interface IAccessTokenGenerationService
{
    string CreateToken(long userId);
}

