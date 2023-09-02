namespace ShortURLGenerator.TelegramBot.Services.Telegram;

/// <summary>Service for sending Telegram messages to a bot.</summary>
public class TelegramBot : TelegramBotClient, ITelegramBot
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Telegram bot object initialization.</summary>
    /// <param name="logger">Log service.</param>
    public TelegramBot(ILogger<TelegramBot> logger) : base(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")!)
    {
        _logger = logger;
    }

    /// <summary>Method for sending a welcome message to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="firstName">First name of user.</param>
    public async Task SendHelloAsync(long chatId, string firstName)
    {
        string head = string.IsNullOrWhiteSpace(firstName) ? "О" : $"{firstName}, о";
        string message = $"{head}тправьте сообщение со ссылкой для генерирования короткого адреса.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    /// <summary>Method for sending the generated link to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="uri">Generated URI.</param>
    /// <param name="sourceUri">Source URI.</param>
    /// <returns>Message ID.</returns>
    public async Task<int> SendUriAsync(long chatId, string uri, string sourceUri)
    {
        if (!string.IsNullOrWhiteSpace(sourceUri))
            uri += $"\nИсходная ссылка:\n{sourceUri}";

        var message = await this.SendTextMessageAsync(chatId: chatId, text: uri);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}\n\tMessage ID: {message.MessageId}\n\tSource URI: {sourceUri}");
        return message.MessageId;
    }

    /// <summary>Method for sending the generated link to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="uri">Generated Url.</param>
    /// <param name="sourceMessageId">Source message ID.</param>
    /// <returns>Message ID.</returns>
    public async Task<int> SendUriAsync(long chatId, string uri, int sourceMessageId)
    {
        var message = await this.SendTextMessageAsync(chatId: chatId, text: uri, replyToMessageId: sourceMessageId);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}\n\tMessage ID: {message.MessageId}\n\tSource message ID: {sourceMessageId}");
        return message.MessageId;
    }

    /// <summary>Method for sending verification code to chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="code">Verification code</param>
    /// <param name="lifeTimeMinutes">Verification code lifetime in minutes.</param>
    public async Task SendVerificationCodeAsync(long chatId, string code, int lifeTimeMinutes)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(lifeTimeMinutes);
        string message = $"Ваш проверочный код для авторизации на сайте: {code}.\n" +
            $"Проверочный код перестанет действовать {deadline.ToString("dd.MM.yyyy HH:mm:ss")}, " +
            "после авторизации или после создания нового кода.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    /// <summary>Method for sending a list of active connections to a chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="connectionsPage">Page with a list of active connections.</param>
    public async Task SendConnectionsAsync(long chatId, ConnectionsPageDto connectionsPage)
    {
        if (connectionsPage is null)
        {
            _logger.LogError($"Failed send connections to chat.\n\tChat ID: {chatId}\n\tError: Connections page is null");
            await SendErrorMessageAsync(chatId, "Не удалось получить текущие подключения.");
            return;
        }

        if (connectionsPage.PageInfo.Count == 0)
        {
            string message = "У Вас нет активных подключений.";
            await this.SendTextMessageAsync(chatId: chatId, text: message);
            _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
            return;
        }

        foreach (var connection in connectionsPage.Connections)
            await SendConnectionAsync(chatId, connection);

        await SendSwitchPage(chatId, connectionsPage.PageInfo.Index, connectionsPage.PageInfo.Count);
    }

    /// <summary>The method for sending a message to close the connection to a chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">The ID of the original message with connection information.</param>
    public async Task SendCloseConnectionAsync(long chatId, int messageId)
    {
        try
        {
            await this.EditMessageTextAsync(
                    chatId,
                    messageId,
                    "Подключение завершено.",
                    replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[0]));

            _logger.LogInformation($"Send close connection to chat.\n\tChat ID: {chatId}\n\tMessage ID: {messageId}");
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, $"Send close connection to chat: failed.\n\tChat ID: {chatId}\n\tMessage ID: {messageId}\n\tError: {ex.Message}");
        }
    }

    /// <summary>Method of sending QR code to chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">Source message ID.</param>
    /// <param name="data">QR code data.</param>
    /// <exception cref="ArgumentNullException">QR code data is null.</exception>
    public async Task SendQRCodeAsync(long chatId, int messageId, byte[] data)
    {
        if (data is null)
        {
            _logger.LogError($"Send QR-Code to chat: failed.\n\tChat ID: {chatId}\n\tMessage ID: {messageId}\n\tError: QRCode data is null");
            await SendErrorMessageAsync(chatId, "Не удалось получить QR-код.");
            return;
        }

        using var stream = new MemoryStream(data);
        await this.SendPhotoAsync(chatId, InputFile.FromStream(stream));
        _logger.LogInformation($"Send QR-Code to chat.\n\tChat ID: {chatId}\n\tMessage ID: {messageId}");
    }

    /// <summary>Method for sending an error message to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="errorMessage">Message describing the error.</param>
    public async Task SendErrorMessageAsync(long chatId, string? errorMessage = null)
    {
        string message = $"Ошибка: {errorMessage ?? "Неизвестная ошибка."} Попробуйте снова.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    /// <summary>Method for sending information about an active connection.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="connection">Connection.</param>
    private async Task SendConnectionAsync(long chatId, ConnectionDto connection)
    {
        var lastActive = DateTime.UtcNow - TimeSpan.FromSeconds(connection.ActiveSecondsAgo);
        var message = $"Последняя активность: {lastActive.ToString("dd.MM.yyyy HH:mm:ss")}\n"
            + $"ОС: {connection.ConnectionInfo.Os ?? "Неизвестно"}\n"
            + $"Браузер: {connection.ConnectionInfo.Browser ?? "Неизвестно"}\n"
            + $"Локация: {connection.ConnectionInfo.Location ?? "Неизвестно"}\n"
            + $"IP: {connection.ConnectionInfo.Ip ?? "Неизвестно"}";

        var key = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: "Завершить", callbackData: $"close_{connection.ConnectionId}"));
        await this.SendTextMessageAsync(chatId: chatId, text: message, replyMarkup: key);
        _logger.LogInformation($"Send connection.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    /// <summary>The method to send a page switcher with active connections.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="index">Page index.</param>
    /// <param name="count">The number of connections per page.</param>
    private async Task SendSwitchPage(long chatId, int index, int count)
    {
        var back = InlineKeyboardButton.WithCallbackData(text: "<<", callbackData: $"connections_{index - 1}");
        var next = InlineKeyboardButton.WithCallbackData(text: ">>", callbackData: $"connections_{index + 1}");
        InlineKeyboardMarkup keyboard;
        if (index == 0)
            keyboard = new InlineKeyboardMarkup(next);
        else if (index == count - 1)
            keyboard = new InlineKeyboardMarkup(back);
        else
            keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[] { back, next });

        string message = $"Страница {index + 1} из {count}";
        await this.SendTextMessageAsync(chatId: chatId, text: message, replyMarkup: keyboard);
        _logger.LogInformation($"Send switch page.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }
}

