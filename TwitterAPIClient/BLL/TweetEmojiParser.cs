using System;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi.Models.V2;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.BLL
{
    public class TweetEmojiParser : ITweetParser
    {
        private readonly ITweetHelper _tweetHelper;
        private readonly ITweetStats _tweetStats;
        private readonly IClientError _error;

        public TweetEmojiParser(ITweetHelper tweetHelper, ITweetStats tweetStats, IClientError error)
        {
            _tweetHelper = tweetHelper;
            _tweetStats = tweetStats;
            _error = error;
        }

        public void Parse(TweetV2 tweet)
        {
            MatchCollection emojis = Regex.Matches(tweet.Text, Constants.EmojiRegEx);
            _tweetHelper.AddToTweetCounter(TweetObjectType.Emoji);

            foreach (var emoji in emojis)
            {
                try
                {
                    if (_tweetStats.TweetObjectList != null)
                    {
                        TweetObjects objOld = _tweetStats.TweetObjectList
                            .Where(c => c.TweetObjType == TweetObjectType.Emoji)
                            .FirstOrDefault(x => x.TweetObjValue == emoji.ToString());
                        if (objOld != null)
                        {
                            objOld.TotalObjCount++;
                        }
                        else
                        {
                            _tweetHelper.AddTweetObject(emoji.ToString(), TweetObjectType.Emoji);
                        }
                    }
                    else
                    {
                        _tweetHelper.AddTweetObject(emoji.ToString(), TweetObjectType.Emoji);
                    }
                }
                catch (Exception e)
                {
                    _error.LogError("TweetEmojiParser.Parse()", e.Message, e, ErrorSeverity.LogError);
                }
            }
        }
    }
}
