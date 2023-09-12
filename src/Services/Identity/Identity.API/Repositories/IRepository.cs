using ShortURLGenerator.Identity.API.Models;

namespace ShortURLGenerator.Identity.API.Repositories;

public interface IRepository<T, TKey> where T : IModel<TKey>
{
    Task<T> CreateAsync(T item);

    Task<T> GetAsync(TKey id);

    Task RemoveAsync(TKey id);
}
