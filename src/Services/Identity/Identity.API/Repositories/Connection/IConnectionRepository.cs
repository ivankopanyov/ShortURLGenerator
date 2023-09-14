namespace ShortURLGenerator.Identity.API.Repositories.Connection;

/// <summary>Repository of connections.</summary>
public interface IConnectionRepository
{
    /// <summary>Method for obtaining a connection by identifier.</summary>
    /// <param name="id">Connection ID.</param>
    /// <returns>
    /// Connection with the passed identifier.
    /// If the connection is not found, null will be returned.
    /// </returns>
    Task<Models.Connection?> GetOrDefaultAsync(string id);

    /// <summary>method for obtaining user connections in a given range.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="index">Index of the page with connections.</param>
    /// <param name="size">Number of connections on the page.</param>
    /// <returns>Connections page.</returns>
    Task<ConnectionsPageDto> GetByUserIdAsync(string userId, int index, int size);

    /// <summary>Method for adding a new connection to the repository.</summary>
    /// <param name="item">New connection.</param>
    /// <returns>Created connection.</returns>
    Task<Models.Connection> CreateAsync(Models.Connection item);

    /// <summary>Method for removing a connection from the repository.</summary>
    /// <param name="id">Connection ID.</param>
    Task RemoveAsync(string id);

    /// <summary>Method for checking whether a repository is connected.</summary>
    /// <param name="id">Connection ID.</param>
    /// <returns>Result of checking.</returns>
    Task<bool> ContainsAsync(string id);
}
