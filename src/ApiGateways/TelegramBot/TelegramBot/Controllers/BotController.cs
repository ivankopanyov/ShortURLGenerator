namespace ShortURLGenerator.TelegramBot.Controllers;

[ApiController]
[Route("/")]
public class BotController : ControllerBase
{
    private readonly IUpdateHandler _updateHandler;

    private readonly ILogger<BotController> _logger;

    public BotController(IUpdateHandler updateHandler,
        ILogger<BotController> logger)
    {
        _updateHandler = updateHandler;
        _logger = logger;
    }

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