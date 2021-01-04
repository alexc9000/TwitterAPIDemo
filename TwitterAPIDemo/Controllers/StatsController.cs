using Microsoft.AspNetCore.Mvc;
using System;
using static TwitterAPIDemo.Helpers.CalculateHelper;
using Microsoft.Extensions.Configuration;
using TwitterAPIDemo.Models;
using TwitterAPIDemo.Interfaces;
using TwitterAPIClient.Interfaces;

namespace TwitterAPIDemo.Controllers
{
    public class StatsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ICalculateHelper _helper;
        private readonly IClientError _clientError;

        public StatsController(IConfiguration config, ICalculateHelper helper, IClientError clientError)
        {
            _config = config;
            _helper = helper;
            _clientError = clientError;
        }

        public IActionResult Index()
        {
            Int32.TryParse(_config["TopResults"], out int TopResults);

            ErrorModel.ErrorList = _clientError.GetErrorlist();

            TweetStatistics stats = new TweetStatistics()
            {
                //Each Individual Tweet
                TotalTweetCount = _helper.GetTotalTweetCount(),

                //Based from TotalTweetCount
                TotalTweetsPerHour = _helper.GetTweetsPerHour(),
                TotalTweetsPerMinute = _helper.GetTweetsPerMinute(),
                TotalTweetsPerSecond = _helper.GetTweetsPerSecond(),

                //By Object
                TopEmojis = _helper.GetTopTweetObjects(TopResults, TweetObjectTypes.Emoji),
                TopHashTags = _helper.GetTopTweetObjects(TopResults, TweetObjectTypes.Hashtag),
                TopDomains = _helper.GetTopTweetObjects(TopResults, TweetObjectTypes.Domain),

                //By Tweet - with one or more of the object type
                PercentOfTweetsWithEmojis = _helper.GetPercentOfTweetsWithObject(TweetObjectTypes.Emoji),
                PercentOfTweetsWithURL = _helper.GetPercentOfTweetsWithObject(TweetObjectTypes.Url),
                PercentOfTweetsWithPhotoURL = _helper.GetPercentOfTweetsWithObject(TweetObjectTypes.PhotoUrl),

                ErrorList = ErrorModel.GetErrorlist().ConvertAll(x => x.ErrorMessage)
            };

            return View(stats);
        }
    }
}
