namespace ShortURLGenerator.URLGenerator.API.Services.URL;

public class UrlService : Grpc.Services.UrlService.UrlServiceBase, IUrlService
{
    private readonly IGeneratable _generator;

    private readonly IUrlRepository _repository;

    private readonly ILogger<UrlService> _logger;

    public UrlService(IGeneratable generator, IUrlRepository repository, ILogger<UrlService> logger)
	{
        _generator = generator;
        _repository = repository;
        _logger = logger;
	}

    public override async Task<UrlResponseDto> Generate(SourceUriDto request, Grpc.Core.ServerCallContext context)
    {
        _logger.LogInformation($"Generate URL: start.\n\tSource URI: {request.Value}");

        while (true)
        {
            var url = _generator.GenerateString();

            try
            {
                await _repository.CreateAsync(new Url()
                {
                    Id = url,
                    SourceUri = request.Value
                });

                _logger.LogInformation($"Generate URL: succesful.\n\tSource URI: {request.Value}\n\tURL: {url}");

                return new UrlResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.Ok
                    },
                    Url = url
                };
            }
            catch (DuplicateWaitObjectException ex)
            {
                _logger.LogWarning(ex, $"Generate URL: failed.\n\tSource URI: {request.Value}\n\tURL: {url}\n\tWarning: duplicate.");
            }
        }
    }
}

