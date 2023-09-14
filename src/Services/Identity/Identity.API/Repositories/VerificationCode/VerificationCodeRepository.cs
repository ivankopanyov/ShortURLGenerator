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

    public async Task<Models.VerificationCode> CreateAsync(Models.VerificationCode item)
    {
        _logger.LogInformation($"Create verification code: Start. Verification code: {item?.LogInfo()}.");

        if (item is null)
        {
            _logger.LogError($"Create verification code: Verification code is null.");
            throw new ArgumentNullException(nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            _logger.LogError($"Create verification code: Verification code ID is null or whitespace. Verification code: {item.LogInfo()}.");
            throw new ArgumentException("Verification code ID is null or whitespace.", nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.UserId))
        {
            _logger.LogError($"Create verification code: User ID is null or whitespace. Verification code: {item.LogInfo()}.");
            throw new ArgumentException("User ID is null or whitespace.", nameof(item));
        }

        if (await ContainsAsync(item.Id))
        {
            _logger.LogError($"Create verification code: Verification code ID is already exists. Verification code: {item.LogInfo()}.");
            throw new InvalidOperationException("Verification code ID is already exists.");
        }

        item.LifeTime = _verificationCodeLifeTime;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _verificationCodeLifeTime
        };

        await _distributedCache.SetStringAsync(item.Id, item.UserId, options);
        await _distributedCache.SetStringAsync($"{PREFIX}{item.UserId}", item.Id, options);

        _logger.LogInformation($"Create verification code: Succesfully. Verification code: {item.LogInfo()}.");

        return item;
    }

    public async Task RemoveByUserIdAsync(string userId)
    {
        _logger.LogInformation($"Remove verification code by user ID: Start. User ID: {userId}.");

        string prefixedUserId = $"{PREFIX}{userId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } verificationCode)
        {
            _logger.LogInformation($"Remove verification code by user ID: Verification code not found. User ID: {userId}.");
            return;
        }

        await _distributedCache.RemoveAsync(prefixedUserId);
        await _distributedCache.RemoveAsync(verificationCode);

        _logger.LogInformation($"Remove verification code by user ID: Succesfully. Verification code: {verificationCode}.");
    }

    public async Task<bool> ContainsAsync(string id) => await _distributedCache.GetStringAsync(id) is not null;

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
