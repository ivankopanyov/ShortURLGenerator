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

    public async Task<TimeSpan> CreateOrUpdateAsync(string userId, string verificationCode)
    {
        _logger.LogStart("Create or update verification code", verificationCode);

        if (_distributedCache.GetStringAsync(verificationCode) != null)
        {
            _logger.LogError("Create or update verification code", "Duplicate", verificationCode);
            throw new DuplicateWaitObjectException(nameof(verificationCode));
        }

        string prefixedUserId = $"{PREFIX}{userId}";

        var code = await _distributedCache.GetStringAsync(prefixedUserId);
        if (code != null)
        {
            await _distributedCache.RemoveAsync(prefixedUserId);
            await _distributedCache.RemoveAsync(code);
        }   

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _verificationCodeLifeTime
        };

        await _distributedCache.SetStringAsync(verificationCode, userId, options); 
        await _distributedCache.SetStringAsync(prefixedUserId, verificationCode, options);

        _logger.LogSuccessfully("Create or update verification code", verificationCode);
        
        return _verificationCodeLifeTime;
    }

    public async Task<string?> GetAndRemoveAsync(string verificationCode)
    {
        _logger.LogStart("Create or update verification code", verificationCode);

        var userId = await _distributedCache.GetStringAsync(verificationCode);
        
        if (userId != null)
        {
            await _distributedCache.RemoveAsync($"{PREFIX}{userId}");
            await _distributedCache.RemoveAsync(verificationCode);
        }

        _logger.LogSuccessfully("Create or update verification code", verificationCode);

        return userId;
    }

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
