namespace ShortURLGenerator.URLGenerator.API.Models.Abstract;

public interface IModel<TKey>
{
    TKey Id { get; }
}

