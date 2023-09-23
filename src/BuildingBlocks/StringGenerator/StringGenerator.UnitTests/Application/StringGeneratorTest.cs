namespace ShortURLGenerator.StringGenerator.UnitTests.Application;

public class StringGeneratorTest
{
    private const string DEFAULT_SOURCE_SYMBOLS = "0123456789";

    private const int DEFAULT_STRING_LENGTH = 1;

    [Fact]
    public void Generate_string_success()
    {
        for (int i = 0; i < 100; i++)
        {
            var stringBuilder = new StringBuilder();
            var length = Random.Shared.Next(1, 100);
            for (int j = 0; j < length; j++)
                stringBuilder.Append((char)Random.Shared.Next(char.MinValue, char.MaxValue));

            var configuration = new StringGeneratorConfiguration()
            {
                StringLength = Random.Shared.Next(1, 100),
                SourceSymbols = stringBuilder.ToString()
            };

            var stringGenerator = new StringGenerator(configuration);
            var result = stringGenerator.GenerateString();

            Assert.Equal(configuration.StringLength, result.Length);

            foreach (var symbol in result)
                Assert.True(configuration.SourceSymbols.Contains(symbol));
        }
    }

    [Fact]
    public void Generate_string_configuration_is_null_success()
    {
        StringGeneratorConfiguration configuration = null!;

        var stringGenerator = new StringGenerator(configuration);
        var result = stringGenerator.GenerateString();

        Assert.Equal(DEFAULT_STRING_LENGTH, result.Length);

        foreach (var symbol in result)
            Assert.True(DEFAULT_SOURCE_SYMBOLS.Contains(symbol));
    }

    [Fact]
    public void Generate_string_bad_length_success()
    {
        for (int i = 0; i < 100; i++)
        {
            var stringBuilder = new StringBuilder();
            var length = Random.Shared.Next(1, 100);
            for (int j = 0; j < length; j++)
                stringBuilder.Append((char)Random.Shared.Next(char.MinValue, char.MaxValue));

            var configuration = new StringGeneratorConfiguration()
            {
                StringLength = Random.Shared.Next(-100, 1),
                SourceSymbols = stringBuilder.ToString()
            };

            var stringGenerator = new StringGenerator(configuration);
            var result = stringGenerator.GenerateString();

            Assert.Equal(DEFAULT_STRING_LENGTH, result.Length);

            foreach (var symbol in result)
                Assert.True(configuration.SourceSymbols.Contains(symbol));
        }
    }

    [Fact]
    public void Generate_string_source_symbols_is_null_success()
    {
        for (int i = 0; i < 100; i++)
        {
            var configuration = new StringGeneratorConfiguration()
            {
                StringLength = Random.Shared.Next(1, 100),
                SourceSymbols = null!
            };

            var stringGenerator = new StringGenerator(configuration);
            var result = stringGenerator.GenerateString();

            Assert.Equal(configuration.StringLength, result.Length);

            foreach (var symbol in result)
                Assert.True(DEFAULT_SOURCE_SYMBOLS.Contains(symbol));
        }
    }

    [Fact]
    public void Generate_string_source_symbols_is_empty_success()
    {
        for (int i = 0; i < 100; i++)
        {
            var configuration = new StringGeneratorConfiguration()
            {
                StringLength = Random.Shared.Next(1, 100),
                SourceSymbols = string.Empty
            };

            var stringGenerator = new StringGenerator(configuration);
            var result = stringGenerator.GenerateString();

            Assert.Equal(configuration.StringLength, result.Length);

            foreach (var symbol in result)
                Assert.True(DEFAULT_SOURCE_SYMBOLS.Contains(symbol));
        }
    }
}

