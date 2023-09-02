namespace ShortURLGenerator.TelegramBot.Controllers;

/// <summary>Telegram bot controller.</summary>
[ApiController]
[Route("/")]
public class BotController : ControllerBase
{
    /// <summary>Telegram update handler.</summary>
    private readonly IUpdateHandler _updateHandler;

    /// <summary>Log service.</summary>
    private readonly ILogger<BotController> _logger;

    /// <summary>Controller initialization.</summary>
    /// <param name="updateHandler">Telegram update handler.</param>
    /// <param name="logger">Log service.</param>
    public BotController(IUpdateHandler updateHandler,
        ILogger<BotController> logger)
    {
        _updateHandler = updateHandler;
        _logger = logger;
    }

    /// <summary>Telegram bot update method using webhook.</summary>
    /// <param name="update">Telegram bot update.</param>
    [HttpPost]
    public async void HandleUpdateAsync(Update update)
    {
        try
        {
            await _updateHandler.HandleAsync(update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}