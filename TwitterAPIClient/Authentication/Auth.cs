using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Exceptions;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;

namespace TwitterAPIClient.Authentication
{
    public class Auth : IAuth
    {
        private readonly IConfiguration _config;
        private readonly IClientError _error;

        public Auth(IConfiguration config, IClientError error)
        {
            _config = config;
            _error = error;
        }

        public async Task<TwitterClient> Authenticate()
        {
            TwitterClient appClient = new TwitterClient(_config["Config:ConsumerKey"], _config["Config:ConsumerSecret"], _config["Config:BearerToken"]);

            try
            {
                await appClient.Auth.RequestAuthenticationUrlAsync();
            }
            catch (TwitterAuthException e)
            {
                _error.LogError("Auth.Authenticate()", e.Message, e, ErrorSeverity.LogError);
                appClient = null;
            }
            catch (TwitterException e)
            {
                _error.LogError("Auth.Authenticate()", e.TwitterDescription, e, ErrorSeverity.LogError);
                appClient = null;
            }
            catch (Exception e)
            {
                _error.LogError("Auth.Authenticate()", e.Message, e, ErrorSeverity.LogError);
                appClient = null;
            }

            return appClient;
        }
    }
}
