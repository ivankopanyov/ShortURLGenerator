namespace ShortURLGenerator.TelegramBot.Services.Identity;

public interface IIdentityService
{
    Task<VerificationCodeDto> GetVerificationCodeAsync(long userId);

    Task<ConnectionsPageDto> GetConnectionsAsync(long userId, int index, int size);

    Task CloseConnectionAsync(long userId, string connectionId);
}

