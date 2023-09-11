namespace ShortURLGenerator.Identity.API.Repositories.Connection;

public interface IConnectionRepository
{
    Task CreateAsync(string userId, string connectionId, ConnectionInfoDto connectionInfo);

    Task<ConnectionsPageDto> GetAsync(string userId, int index, int size);
}
