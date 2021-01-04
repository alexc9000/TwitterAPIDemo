using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TwitterAPIClient.Models;

namespace TwitterAPIDemo.Models
{
    public class ErrorModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public static List<ClientErrorInfo> ErrorList { get; set; } = new List<ClientErrorInfo>();

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

        public static List<ClientErrorInfo> GetErrorlist()
        {
            return ErrorList;
        }
    }
}
