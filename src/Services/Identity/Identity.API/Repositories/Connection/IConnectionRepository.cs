namespace ShortURLGenerator.Identity.API.Repositories.Connection;

public interface IConnectionRepository
{
    Task<Models.Connection?> GetOrDefaultAsync(string id);

    Task<ConnectionsPageDto> GetByUserIdAsync(string userId, int index, int size);

    Task<Models.Connection> CreateAsync(Models.Connection item);

    Task RemoveAsync(string id);

    Task<bool> ContainsAsync(string id);
}
