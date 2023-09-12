namespace ShortURLGenerator.Identity.API.Repositories.Connection;

public interface IConnectionRepository
{
    Task<Models.Connection?> GetAsync(string connectionId);

    Task<ConnectionsPageDto> GetAsync(string userId, int index, int size);

    Task<Models.Connection?> CreateAsync(Models.Connection connection);

    Task RemoveAsync(string connectionId);

    Task<bool> Contains(string id);
}
