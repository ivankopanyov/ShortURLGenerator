namespace ShortURLGenerator.Web.Bff.UrlGenerator.Controllers;

/// <summary>Сlass that describes a authentication controller.</summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    /// <summary>User identity service.</summary>
    private readonly IIdentityService _identityService;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Initializing the controller object.</summary>
    /// <param name="identityService">User identity service.</param>
    /// <param name="logger">Log service.</param>
    public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    /// <summary>Endpoint for connecting a telegram bot to a site.</summary>
    /// <param name="singIn">The request object to connect.</param>
    /// <returns>Access tokens.</returns>
    [HttpPost("signIn")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Token>> SignInAsync([FromBody] SingInDto singIn)
    {
        _logger.LogInformation($"Sign in: Start. {singIn}.");

        if (string.IsNullOrWhiteSpace(singIn.VerificationCode))
        {
            _logger.LogError($"Sign in: Verification code is null or whitespace. {singIn}.");
            return BadRequest("Не указан проверочный код.");
        }

        try
        {
            var token = await _identityService.SignInAsync(singIn.VerificationCode, singIn.ConnectionInfo);
            _logger.LogInformation($"Sign in: Successfully. {token.LogInfo()}.");

            return Ok(token);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"Sign in: {ex.Message}.");
            return NotFound(ex.Message);
        }
    }

    /// <summary>Endpoint for closing the connection of a telegram bot to a site.</summary>
    /// <param name="connectionId">Connection ID or refresh token.</param>
    [HttpDelete("signOut/{connectionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SignOutAsync([FromRoute] string connectionId)
    {
        _logger.LogInformation($"Sign out: Start. Connection ID: {connectionId}.");

        if (string.IsNullOrWhiteSpace(connectionId))
        {
            _logger.LogError($"Sign out: Connection ID is null or whitespace.");
            return BadRequest("Connection ID is null or whitespace.");
        }

        if (User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) is not { } idClaim || !long.TryParse(idClaim.Value, out long userId))
        {
            _logger.LogError($"Sign out: Claim is null or not parsed to long.");
            return Unauthorized();
        }

        try
        {
            await _identityService.CloseConnectionAsync(userId, connectionId);
            _logger.LogInformation($"Sign out: Successfully. User ID: {userId}, Connection ID: {connectionId}.");
            return Ok();
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Sign out: {ex.Message}. User ID: {userId}, Connection ID: {connectionId}.");
            return NotFound(ex.Message);
        }
    }

    /// <summary>Access token refresh endpoint.</summary>
    /// <param name="refreshTokenDto">Connection ID or refresh token.</param>
    /// <returns>New access tokens.</returns>
    [HttpPost("refreshToken")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Token>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        _logger.LogInformation($"Refresh token: Start. {refreshTokenDto}.");

        if (refreshTokenDto?.Token?.AccessToken is not { } accessToken || string.IsNullOrWhiteSpace(accessToken))
        {
            _logger.LogError("Refresh token: Access token is null or whitespace.");
            return BadRequest("Access token is null or whitespace.");
        }

        if (refreshTokenDto?.Token?.RefreshToken is not { } refreshToken || string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogError("Refresh token: Refresh token is null or whitespace.");
            return BadRequest("Refresh token is null or whitespace.");
        }

        if (GetUserId(accessToken) is not long userId)
        {
            _logger.LogError("Refresh token: Access token is not valid.");
            return BadRequest("Access token is not valid.");
        }

        try
        {
            var token = await _identityService.RefreshTokenAsync(userId, refreshToken, refreshTokenDto.ConnectionInfo);
            _logger.LogInformation($"Refresh token: Successfully. {token.LogInfo()}.");

            return Ok(token);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"Refresh token: {ex.Message}.");
            return NotFound(ex.Message);
        }
    }

    /// <summary>Method for obtaining user ID from access token.</summary>
    /// <param name="token">Access token.</param>
    /// <returns>User ID.</returns>
    private long? GetUserId(string token)
    {
        _logger.LogInformation($"Get user id: Start. Access token: {token}.");

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("Get user id: Access token is null or whitespace.");
            return null;
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken)
        {
            _logger.LogError("Get user id: Security token is not JwtSecurityToken.");
            return null;
        }

        if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogError("Get user id: Security token algoritm is not HmacSha256.");
            return null;
        }

        if (principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) is not { } idClaim)
        {
            _logger.LogError("Get user id: Name identifier claim not found.");
            return null;
        }

        if (!long.TryParse(idClaim.Value, out long userId))
        {
            _logger.LogError($"Get user id: User Id is not valid. User ID: {idClaim.Value}");
            return null;
        }

        return userId;
    }
}
