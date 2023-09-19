using ShortURLGenerator.Grpc.Services;

namespace ShortURLGenerator.Web.Bff.UrlGenerator.Controllers;

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

    [HttpPost("signIn")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Token>> SignInAsync([FromBody] SingInDto singIn)
    {
        _logger.LogInformation($"Sign in: Start. {singIn}.");

        if (string.IsNullOrWhiteSpace(singIn.VerificationCode))
            return BadRequest("Не указан проверочный код.");

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

    [HttpDelete("signOut/{connectionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SignOutAsync([FromRoute] string connectionId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("refreshToken")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Token>> RefreshTokenAsync(RefreshTokenDto refreshToken)
    {
        throw new NotImplementedException();
    }
}
