namespace ShortURLGenerator.URLGenerator.UnitTests;

public class UrlContextMock : UrlContext
{
    public static UrlContextMock Empty
    {
        get
        {
            using (var urlContextMock = new UrlContextMock())
            {
                urlContextMock.Urls.RemoveRange(urlContextMock.Urls);
                urlContextMock.SaveChanges();
            };

            return new UrlContextMock();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(databaseName: "UrlsDatabase");
    }
}

