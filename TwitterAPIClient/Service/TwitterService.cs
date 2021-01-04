using System;
using System.Threading.Tasks;
using Tweetinvi;
using Microsoft.Extensions.Logging;
using TwitterAPIClient.Interfaces;
using Tweetinvi.Exceptions;
using TwitterAPIClient.Models;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace TwitterAPIClient.Service
{
    public class TwitterService : BackgroundService
    {
        private readonly ILogger<TwitterService> _logger;
        private readonly IAuth _auth;
        private readonly ITweetActions _tweetActions;
        private readonly IClientError _error;

        public TwitterService(ILogger<TwitterService> logger, IAuth auth, ITweetActions tweetActions, IClientError error)
        {
            _logger = logger;
            _auth = auth;
            _tweetActions = tweetActions;
            _error = error;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Twitter service starting");

            _tweetActions.SetStartTime();

            //Authenticate
            TwitterClient appClient = await _auth.Authenticate();

            // loop until a cancellation is requested
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Twitter service executing - {0}", DateTime.Now);

                if (appClient != null)
                {
                    //Get Sample Stream
                    var sampleStreamV2 = appClient.StreamsV2.CreateSampleStream();
                    sampleStreamV2.TweetReceived += (sender, args) =>
                    {
                        try
                        {
                            _tweetActions.ParseTweetObjects(args.Tweet);
                        }
                        catch (TwitterException e)
                        {
                            _error.LogError("witterService.GetTwitterDataAsync()", e.TwitterDescription, e, ErrorSeverity.LogError);
                            appClient = null;
                        }
                        catch (Exception e)
                        {
                            _error.LogError("witterService.GetTwitterDataAsync()", e.Message, e, ErrorSeverity.LogError);
                            appClient = null;
                        }
                    };

                    await sampleStreamV2.StartAsync();

                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Twitter service stopping");
            await Task.CompletedTask;
        }
    }
}
