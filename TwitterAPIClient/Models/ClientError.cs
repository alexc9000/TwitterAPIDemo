using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TwitterAPIClient.Interfaces;

namespace TwitterAPIClient.Models
{
    public class ClientError : IClientError
    {
        private readonly ILogger<ClientError> _logger;

        private List<ClientErrorInfo> ErrorList { get; set; } = new List<ClientErrorInfo>();

        public ClientError(ILogger<ClientError> logger)
        {
            _logger = logger;
        }

        public void LogError(string source, string errorMessage, Exception error, ErrorSeverity severity)
        {
            ClientErrorInfo errorInfo = new ClientErrorInfo()
            {
                ErrorSource = source,
                ErrorMessage = errorMessage,
                ErrorException = error,
                Severity = severity
            };

            ErrorList.Add(errorInfo);

            switch (severity)
            {
                case ErrorSeverity.LogTrace:
                    _logger.LogTrace(error.Message);
                    break;
                case ErrorSeverity.LogDebug:
                    _logger.LogDebug(error.Message);
                    break;
                case ErrorSeverity.LogInformation:
                    _logger.LogInformation(error.Message);
                    break;
                case ErrorSeverity.LogWarning:
                    _logger.LogWarning(error.Message);
                    break;
                case ErrorSeverity.LogError:
                    _logger.LogError(error.Message);
                    break;
                case ErrorSeverity.LogCritical:
                    _logger.LogCritical(error.Message);
                    break;
                default:
                    _logger.LogError(error.Message);
                    break;
            }
        }

        public List<ClientErrorInfo> GetErrorlist()
        {
            return ErrorList;
        }
    }

    public class ClientErrorInfo
    {
        public string ErrorSource { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public ErrorSeverity Severity { get; set; }
    }

    public enum ErrorSeverity
    {
        LogTrace,
        LogDebug,
        LogInformation,
        LogWarning,
        LogError,
        LogCritical
    }

}
