namespace ShortURLGenerator.URLGenerator.API.Models;

/// <summary>Сlass that describes the generated URL model.</summary>
public class Url
{
    /// <summary>URL identifier.</summary>
    public string Id { get; set; }

    /// <summary>The date and time the URL was created.</summary>
    public DateTime Created { get; set; }

    /// <summary>Source URI address.</summary>
    public string SourceUri { get; set; }

    /// <summary>Overriding the method of casting an object to a string type.</summary>
    /// <returns>Object cast to a string type.</returns>
    public override string ToString() =>
        "URL: { ID: " + Id +
        ", Source URI: " +SourceUri +
        ", Created: " + Created.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " }";
}

