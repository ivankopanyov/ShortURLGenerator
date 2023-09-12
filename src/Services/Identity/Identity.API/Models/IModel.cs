namespace ShortURLGenerator.Identity.API.Models;

public interface IModel<TKey>
{
    public TKey Id { get; }
}
