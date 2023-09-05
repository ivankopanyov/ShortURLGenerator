namespace ShortURLGenerator.URLGenerator.API.Repositories;

public interface IRepository<T, TKey> where T : IModel<TKey>
{
    Task CreateAsync(T item);

    Task<T?> GetAsync(TKey id);
}

