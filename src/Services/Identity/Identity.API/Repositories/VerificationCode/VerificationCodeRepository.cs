namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private const int DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES = 1;

    private const string PREFIX = "code_";

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger _logger;

    private readonly TimeSpan _verificationCodeLifeTime;

	public VerificationCodeRepository(IDistributedCache distributedCache,
        ILogger<VerificationCodeRepository> logger,
        IConfiguration? configuration = null)
    {
        _distributedCache = distributedCache;
        _logger = logger;

        var repositoryConfiguration = new VerificationCodeRepositoryConfiguration();
        OnConfiguring(repositoryConfiguration, configuration);

        _verificationCodeLifeTime = 
            repositoryConfiguration.VerificationCodeLifeTime.Minutes < DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES
                ? TimeSpan.FromMinutes(DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES)
                : repositoryConfiguration.VerificationCodeLifeTime;
    }

    public async Task<Models.VerificationCode?> GetAsync(string id) => 
        string.IsNullOrWhiteSpace(id) || await _distributedCache.GetStringAsync(id) is not { } userId
            ? null : new Models.VerificationCode()
            {
                Id = id,
                UserId = userId,
                LifeTime = _verificationCodeLifeTime
            };

    public async Task<Models.VerificationCode?> CreateAsync(Models.VerificationCode verificationCode)
    {
        if (verificationCode is null ||
            string.IsNullOrWhiteSpace(verificationCode.Id) ||
            string.IsNullOrWhiteSpace(verificationCode.UserId))
            return null;

        verificationCode.LifeTime = _verificationCodeLifeTime;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _verificationCodeLifeTime
        };

        await _distributedCache.SetStringAsync(verificationCode.Id, verificationCode.UserId, options);
        await _distributedCache.SetStringAsync($"{PREFIX}{verificationCode.UserId}", verificationCode.Id, options);

        return verificationCode;
    }

    public async Task RemoveAsync(string id)
    {
        if (await _distributedCache.GetStringAsync(id) is not { } userId)
            return;

        await _distributedCache.RemoveAsync($"{PREFIX}{userId}");
        await _distributedCache.RemoveAsync(id);
    }

    public async Task<bool> Contains(string id) =>
        await _distributedCache.GetStringAsync(id) is not null;

    protected virtual void OnConfiguring(
        VerificationCodeRepositoryConfiguration repositoryConfiguration,
        IConfiguration? appConfiguration = null)
    {
        if (appConfiguration != null)
        {
            var minutes = appConfiguration
                .GetSection("VerificationCode")
                .GetValue<int>("LifeTimeMinutes");

            repositoryConfiguration.VerificationCodeLifeTime = TimeSpan
                .FromMinutes(Math.Max(DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES, minutes));
        }
        else
            repositoryConfiguration.VerificationCodeLifeTime = TimeSpan
                .FromMinutes(DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES);
    }
}
