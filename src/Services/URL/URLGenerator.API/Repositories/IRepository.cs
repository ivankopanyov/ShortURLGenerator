namespace ShortURLGenerator.URLGenerator.API.Repositories;

/// <summary>Entity repository.</summary>
/// <typeparam name="T">Entity type.</typeparam>
/// <typeparam name="TKey">Entity identifier type.</typeparam>
public interface IRepository<T, TKey> where T : IModel<TKey>
{
    /// <summary>Method for adding an entity to the repository.</summary>
    /// <param name="item">Entity.</param>
    Task CreateAsync(T item);

    /// <summary>Method for getting an entity from the repository.</summary>
    /// <param name="id">Entity identifier.</param>
    /// <returns>The requested entity. Returns null if the entity is not found.</returns>
    Task<T?> GetAsync(TKey id);
}

