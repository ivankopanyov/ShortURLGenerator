namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

/// <summary>Access token generation service.</summary>
public interface IAccessTokenGenerationService
{
    /// <summary>Access token generation method.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Access token.</returns>
    string CreateToken(long userId);
}

