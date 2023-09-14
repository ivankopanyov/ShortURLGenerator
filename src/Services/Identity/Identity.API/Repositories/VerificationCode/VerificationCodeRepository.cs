namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

/// <summary>
/// Class describing a verification code repository.
/// Implements the IVerificationCodeRepository interface.
/// IDistributedCache is used to store connections.
/// </summary>
public class VerificationCodeRepository : IVerificationCodeRepository
{
    /// <summary>
    /// The default number of minutes to store a verification code.
    /// Used if the verification code storage period is not set or set incorrectly.
    /// </summary>
    private const int DEFAULT_VERIFICATION_CODE_LIFE_TIME_MINUTES = 1;

    /// <summary>User ID prefix for the key.</summary>
    private const string PREFIX = "code_";

    /// <summary>Distributed cache.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Duration of verification code storage.</summary>
    private readonly TimeSpan _verificationCodeLifeTime;

    /// <summary>Initializing the repository object.</summary>
    /// <param name="distributedCache">Distributed cache.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configurationю.</param>
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

    /// <summary>Method for adding new verification code to the repository.</summary>
    /// <param name="item">New verification code.</param>
    /// <returns>Created verification code.</returns>
    /// <exception cref="ArgumentNullException">Exception is thrown if the verification code is null.</exception>
    /// <exception cref="ArgumentException">Exception is thrown if the verification code ID or user ID is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Exception is thrown if the verification code ID is already exists.</exception>
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

    /// <summary>Method for removing verification code from the repository by user ID.</summary>
    /// <param name="userId">User ID.</param>
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

    /// <summary>Method for checking whether a repository is verification code.</summary>
    /// <param name="id">Verification code ID.</param>
    /// <returns>Result of checking.</returns>
    public async Task<bool> ContainsAsync(string id) => await _distributedCache.GetStringAsync(id) is not null;

    /// <summary>
    /// Virtual method for configuring a repository.
    /// To set the connection lifetime value, use the value of the "LifeTimeMinutes" field
    /// from the "VerificationCode" section of the application configuration.
    /// </summary>
    /// <param name="repositoryConfiguration">Repository configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
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
