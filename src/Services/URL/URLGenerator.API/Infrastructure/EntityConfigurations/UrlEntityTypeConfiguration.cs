namespace ShortURLGenerator.URLGenerator.API.Infrastructure.EntityConfigurations;

public class UrlEntityTypeConfiguration : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.SourceUri)
            .IsRequired(true)
            .HasMaxLength(2048);
    }
}
