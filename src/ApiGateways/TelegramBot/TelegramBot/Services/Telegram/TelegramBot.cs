﻿namespace ShortURLGenerator.TelegramBot.Services.Telegram;

/// <summary>Service for sending Telegram messages to a bot.</summary>
public class TelegramBot : TelegramBotClient, ITelegramBot
{
    /// <summary>The domain name of the site to generate short URLs.</summary>
    private static readonly string _frontend = Environment.GetEnvironmentVariable("FRONTEND")!;

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
    /// <param name="url">Generated Url.</param>
    public async Task SendUriAsync(long chatId, string url)
    {
        string message = $"https://{_frontend}/{url}";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    /// <summary>Method for sending verification code to chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="code">Verification code</param>
    /// <param name="lifeTimeMinutes">Verification code lifetime in minutes.</param>
    public async Task SendVerificationCodeAsync(long chatId, string code, int lifeTimeMinutes)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(lifeTimeMinutes);
        string message = $"Ваш проверочный код для авторизации на сайте {_frontend}: {code}.\n" +
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

