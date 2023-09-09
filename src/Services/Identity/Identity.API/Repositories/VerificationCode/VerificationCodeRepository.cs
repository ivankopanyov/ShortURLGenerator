using Microsoft.Extensions.Caching.Distributed;

namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private const int DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES = 1;

    private const string PREFIX = "code_";

    /// <summary>Cache service.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    private readonly TimeSpan _verificationCodeLifeTime;

    /// <summary>Repository object initialization.</summary>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
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

    public async Task<TimeSpan> CreateAsync(string userId, string verificationCode)
    {
        if (_distributedCache.GetStringAsync(verificationCode) != null)
            throw new DuplicateWaitObjectException(nameof(verificationCode));

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

        return _verificationCodeLifeTime;
    }

    public async Task<string?> GetAndRemoveAsync(string verificationCode)
    {
        var userId = await _distributedCache.GetStringAsync(verificationCode);
        
        if (userId != null)
        {
            await _distributedCache.RemoveAsync($"{PREFIX}{userId}");
            await _distributedCache.RemoveAsync(verificationCode);
        }

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
