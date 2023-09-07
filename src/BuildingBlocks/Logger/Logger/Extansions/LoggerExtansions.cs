namespace ShortURLGenerator.Logger.Extansions;

/// <summary>Static class of logger extensions.</summary>
public static class LoggerExtansions
{
    /// <summary>The current date and time.</summary>
    private static string Now => DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff");

    /// <summary>Method of logging the beginning of the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogStart(this ILogger logger, string operationName, string? objectId = null) =>
        logger.LogInformation($"{Now} {operationName}: start.{GetObjectId(objectId)}");

    /// <summary>Method for logging successful completion of the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogSuccessfully(this ILogger logger, string operationName, string? objectId = null) =>
        logger.LogInformation($"{Now} {operationName}: successfully.{GetObjectId(objectId)}");

    /// <summary>method of logging the process of performing an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="informationMessage">Message about the current state of the operation.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogInformation(this ILogger logger, string operationName, string informationMessage, string? objectId = null) =>
        logger.LogInformation($"{Now} {operationName}: {informationMessage}.{GetObjectId(objectId)}");

    /// <summary>Method for logging a warning that occurred during the execution of an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="warningMessage">The warning message that occurred during the execution of the operation.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogWarning(this ILogger logger, string operationName, string warningMessage, string? objectId = null) =>
        logger.LogWarning($"{Now} {operationName}: warning. {warningMessage}.{GetObjectId(objectId)}");

    /// <summary>Method for logging a warning that occurred during the execution of an operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="ex">The exception that was thrown during the execution of the operation.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="warningMessage">The warning message that occurred during the execution of the operation.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogWarning(this ILogger logger, Exception ex, string operationName, string warningMessage, string? objectId = null) =>
        logger.LogWarning(ex, $"{Now} {operationName}: warning. {warningMessage}.{GetObjectId(objectId)}");

    /// <summary>Method for logging an error that occurred during the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="errorMessage">Message about the error that occurred during the operation.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogError(this ILogger logger, string operationName, string errorMessage, string? objectId = null) =>
        logger.LogWarning($"{Now} {operationName}: error. {errorMessage}.{GetObjectId(objectId)}");

    /// <summary>Method for logging an error that occurred during the operation.</summary>
    /// <param name="logger">Logger.</param>
    /// <param name="ex">The exception that was thrown during the execution of the operation.</param>
    /// <param name="operationName">Operation name.</param>
    /// <param name="errorMessage">Message about the error that occurred during the operation.</param>
    /// <param name="objectId">Operation object ID.</param>
    public static void LogError(this ILogger logger, Exception ex, string operationName, string errorMessage, string? objectId = null) =>
        logger.LogWarning(ex, $"{Now} {operationName}: error. {errorMessage}.{GetObjectId(objectId)}");

    /// <summary>Method to cast the identifier of an object to a displayable string.</summary>
    /// <param name="objectId">Object ID.</param>
    /// <returns>Displayable string.</returns>
    private static string GetObjectId(string? objectId) => objectId is null ? string.Empty : $" Object ID: \"{objectId}\".";
}

