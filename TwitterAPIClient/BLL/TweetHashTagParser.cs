using System;
using System.Linq;
using Tweetinvi.Models.V2;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.BLL
{
    public class TweetHashTagParser : ITweetParser
    {
        private readonly ITweetHelper _tweetHelper;
        private readonly ITweetStats _tweetStats;
        private readonly IClientError _error;

        public TweetHashTagParser(ITweetHelper tweetHelper, ITweetStats tweetStats, IClientError error)
        {
            _tweetHelper = tweetHelper;
            _tweetStats = tweetStats;
            _error = error;
        }

        public void Parse(TweetV2 tweet)
        {
            HashtagV2[] hashtags = tweet.Entities?.Hashtags;
            _tweetHelper.AddToTweetCounter(TweetObjectType.Hashtag);

            foreach (HashtagV2 hashtag in hashtags)
            {
                try
                {
                    if (_tweetStats.TweetObjectList != null)
                    {
                        TweetObjects objOld = _tweetStats.TweetObjectList
                            .Where(c => c.TweetObjType == TweetObjectType.Hashtag)
                            .FirstOrDefault(x => x.TweetObjValue == hashtag.Tag);
                        if (objOld != null)
                        {
                            objOld.TotalObjCount++;
                        }
                        else
                        {
                            _tweetHelper.AddTweetObject(hashtag.Tag, TweetObjectType.Hashtag);
                        }
                    }
                    else
                    {
                        _tweetHelper.AddTweetObject(hashtag.Tag, TweetObjectType.Hashtag);
                    }
                }
                catch (Exception e)
                {
                    _error.LogError("TweetHashTagParser.Parse()", e.Message, e, ErrorSeverity.LogError);
                }
            }
        }
    }
}
