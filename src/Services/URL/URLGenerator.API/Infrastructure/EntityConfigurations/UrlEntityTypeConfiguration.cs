namespace ShortURLGenerator.URLGenerator.API.Infrastructure.EntityConfigurations;

/// <summary>Class that describes the configuration of the entity model of the generated short URL.</summary>
public class UrlEntityTypeConfiguration : IEntityTypeConfiguration<Url>
{
    /// <summary>Model configuration method.</summary>
    /// <param name="builder">Entity type builder.</param>
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.SourceUri)
            .IsRequired(true)
            .HasMaxLength(2048);
    }
}
