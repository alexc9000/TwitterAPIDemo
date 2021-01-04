using System;
using System.Collections.Generic;
using TwitterAPIClient.Models;

namespace TwitterAPIClient.Interfaces
{
    public interface IClientError
    {
        void LogError(string source, string errorMessage, Exception error, ErrorSeverity severity);
        List<ClientErrorInfo> GetErrorlist();
    }
}