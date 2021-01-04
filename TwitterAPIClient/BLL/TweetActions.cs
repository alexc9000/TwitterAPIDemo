using System;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi.Models.V2;
using TwitterAPIClient.Interfaces;

namespace TwitterAPIClient.BLL
{
    public class TweetActions : ITweetActions
    {
        private readonly ITweetHelper _tweetHelper;
        private readonly ITweetStats _tweetStats;
        private readonly IClientError _error;

        public TweetActions(ITweetHelper tweetHelper, ITweetStats tweetStats, IClientError error)
        {
            _tweetHelper = tweetHelper;
            _tweetStats = tweetStats;
            _error = error;
        }

        public void SetStartTime()
        {
            _tweetStats.StartTime = DateTime.Now;
        }

        public void ParseTweetObjects(TweetV2 tweet)
        {
            bool hasTweetObject = false;

            ITweetParser tweetParser;
            UrlV2[] urls = tweet.Entities?.Urls;
            HashtagV2[] hashtags = tweet.Entities?.Hashtags;
            MatchCollection emojis = Regex.Matches(tweet.Text, Constants.EmojiRegEx);

            if (hashtags != null && hashtags.Any())
            {
                hasTweetObject = true;
                tweetParser = new TweetHashTagParser(_tweetHelper, _tweetStats, _error);
                tweetParser.Parse(tweet);
            }

            if (emojis != null && emojis.Any())
            {
                hasTweetObject = true;
                tweetParser = new TweetEmojiParser(_tweetHelper, _tweetStats, _error);
                tweetParser.Parse(tweet);
            }

            if (urls != null && urls.Any())
            {
                hasTweetObject = true;
                tweetParser = new TweetURLParser(_tweetHelper, _tweetStats, _error);
                tweetParser.Parse(tweet);
            }

            if (!hasTweetObject)
            {
                tweetParser = new TweetWithoutObjectParser(_tweetHelper);
                tweetParser.Parse(tweet);
            }
        }
    }
}
