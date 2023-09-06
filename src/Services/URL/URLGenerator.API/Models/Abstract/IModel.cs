namespace ShortURLGenerator.URLGenerator.API.Models.Abstract;

/// <summary>Entity model interface.</summary>
/// <typeparam name="TKey">Key type.</typeparam>
public interface IModel<TKey>
{
    /// <summary>Entity identifier.</summary>
    TKey Id { get; }
}

