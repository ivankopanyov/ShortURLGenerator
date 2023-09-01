using System;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShortURLGenerator.TelegramBot.Services.Telegram;

public class TelegramBot : TelegramBotClient, ITelegramBot
{
    private static readonly string _frontend = Environment.GetEnvironmentVariable("FRONTEND")!;

    private readonly ILogger _logger;

    public TelegramBot(ILogger<TelegramBot> logger) : base(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")!)
    {
        _logger = logger;
    }

    public async Task SendHelloAsync(long chatId, string firstName)
    {
        string head = string.IsNullOrWhiteSpace(firstName) ? "О" : $"{firstName}, о";
        string message = $"{head}тправьте сообщение со ссылкой для генерирования короткого адреса.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    public async Task SendUriAsync(long chatId, string url)
    {
        string message = $"https://{_frontend}/{url}";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    public async Task SendVerificationCodeAsync(long chatId, string code, int lifeTimeMinutes)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(lifeTimeMinutes);
        string message = $"Ваш проверочный код для авторизации на сайте {_frontend}: {code}.\n" +
            $"Проверочный код перестанет действовать {deadline.ToString("dd.MM.yyyy HH:mm:ss")}, " +
            "после авторизации или после создания нового кода.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

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

    public async Task SendErrorMessageAsync(long chatId, string? errorMessage = null)
    {
        string message = $"Ошибка: {errorMessage ?? "Неизвестная ошибка."} Попробуйте снова.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    private async Task SendConnectionAsync(long userId, ConnectionDto connection)
    {
        var lastActive = DateTime.UtcNow - TimeSpan.FromSeconds(connection.ActiveSecondsAgo);
        var message = $"Последняя активность: {lastActive.ToString("dd.MM.yyyy HH:mm:ss")}\n"
            + $"ОС: {connection.ConnectionInfo.Os ?? "Неизвестно"}\n"
            + $"Браузер: {connection.ConnectionInfo.Browser ?? "Неизвестно"}\n"
            + $"Локация: {connection.ConnectionInfo.Location ?? "Неизвестно"}\n"
            + $"IP: {connection.ConnectionInfo.Ip ?? "Неизвестно"}";

        var key = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: "Завершить", callbackData: $"close_{connection.ConnectionId}"));
        await this.SendTextMessageAsync(chatId: userId, text: message, replyMarkup: key);
        _logger.LogInformation($"Send connection.\n\tChat ID: {userId}\n\tMessage: {message}");
    }

    private async Task SendSwitchPage(long userId, int index, int count)
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
        await this.SendTextMessageAsync(chatId: userId, text: message, replyMarkup: keyboard);
        _logger.LogInformation($"Send switch page.\n\tChat ID: {userId}\n\tMessage: {message}");
    }
}

