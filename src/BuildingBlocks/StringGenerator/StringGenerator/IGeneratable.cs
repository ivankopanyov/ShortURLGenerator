namespace ShortURLGenerator.StringGenerator;

/// <summary>Random string generator.</summary>
public interface IGeneratable
{
    /// <summary>Method for generating a random string.</summary>
    /// <returns>Random string.</returns>
    string GenerateString();
}