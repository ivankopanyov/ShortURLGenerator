namespace ShortURLGenerator.Logger.Extansions;

/// <summary>Static class of logger extensions.</summary>
public static class LoggerExtansions
{
    /// <summary>Method of logging the beginning of the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogStart(this ILogger logger, string operationName, params object[] objects) =>
        logger.LogInformation($"{operationName}: start.\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>Method for logging successful completion of the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogSuccessfully(this ILogger logger, string operationName, params object[] objects) =>
        logger.LogInformation($"{operationName}: successfully.\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>method of logging the process of performing an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="informationMessage">Message about the current state of the operation.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogInformation(this ILogger logger, string operationName, string informationMessage, params object[] objects) =>
        logger.LogInformation($"{operationName}: {informationMessage}\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>Method for logging a warning that occurred during the execution of an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="warningMessage">The warning message that occurred during the execution of the operation.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogWarning(this ILogger logger, string operationName, string warningMessage, params object[] objects) =>
        logger.LogWarning($"{operationName}: warning. {warningMessage}\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>Method for logging a warning that occurred during the execution of an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="ex">The exception that was thrown during the execution of the operation.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="warningMessage">The warning message that occurred during the execution of the operation.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogWarning(this ILogger logger, Exception ex, string operationName, string warningMessage, params object[] objects) =>
        logger.LogWarning(ex, $"{operationName}: warning. {warningMessage}\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>Method for logging an error that occurred during the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="errorMessage">Message about the error that occurred during the operation.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogError(this ILogger logger, string operationName, string errorMessage, params object[] objects) =>
        logger.LogWarning($"{operationName}: error. {errorMessage}\n{string.Join('\n', objects ?? Array.Empty<object>())}");

    /// <summary>Method for logging an error that occurred during the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="ex">The exception that was thrown during the execution of the operation.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="errorMessage">Message about the error that occurred during the operation.</param>
    /// <param name="objects">Operation targets.</param>
    public static void LogError(this ILogger logger, Exception ex, string operationName, string errorMessage, params object[] objects) =>
        logger.LogWarning(ex, $"{operationName}: error. {errorMessage}\n{string.Join('\n', objects ?? Array.Empty<object>())}");
}

