using System;
using System.Collections.Generic;
using System.Linq;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using TwitterAPIDemo.Interfaces;

namespace TwitterAPIDemo.Helpers
{
    public class CalculateHelper : ICalculateHelper
    {
        private int _totalTweets;
        private double _tweetsPerSecond;
        private double _tweetsPerMinute;
        private double _tweetsPerHour;

        private readonly List<TweetObjects> _tweetObjects = new List<TweetObjects>();
        private readonly List<TweetCounts> _tweetCounts = new List<TweetCounts>();
        private readonly IClientError _error;
        private readonly DateTime _startTime;

        public CalculateHelper(ITweetStats tweetStats, IClientError error)
        {
            _startTime = tweetStats.StartTime;
            _error = error;
            _tweetObjects = tweetStats.TweetObjectList;
            _tweetCounts = tweetStats.TweetCountList;
        }

        public enum TweetObjectTypes
        {
            Hashtag,
            Emoji,
            Domain,
            Url,
            PhotoUrl,
            NoTrackingObject
        }

        public int GetTotalTweetCount()
        {
            _totalTweets = (from x in _tweetCounts select x.TotalCount).Sum();
            LoadTotalStats();

            return _totalTweets;
        }

        public double GetTweetsPerSecond()
        {
            return _tweetsPerSecond;
        }

        public double GetTweetsPerMinute()
        {
            return _tweetsPerMinute;
        }

        public double GetTweetsPerHour()
        {
            return _tweetsPerHour;
        }

        public List<string> GetTopTweetObjects(int howMany, TweetObjectTypes objType)
        {
            List<string> results = (from e in _tweetObjects
                                    orderby e.TotalObjCount descending
                                    where e.TweetObjType.ToString() == objType.ToString()
                                    select e.TweetObjValue
                     ).Take(howMany).ToList();

            return results;
        }

        public double GetPercentOfTweetsWithObject(TweetObjectTypes objType)
        {
            double result = 0;
            try
            {
                int totalWithObjType = (from x in _tweetCounts where x.TweetObjType.ToString() == objType.ToString() select x.TotalCount).Sum();
                result = (double)Math.Round((double)(100 * totalWithObjType) / _totalTweets, 2);
            }
            catch (Exception e)
            {
                _error.LogError("CalculateHelper.GetPercentOfTweetsWithObject()", e.Message, e, ErrorSeverity.LogError);
            }

            return result;
        }



        private void LoadTotalStats()
        {
            try
            {
                _tweetsPerSecond = Math.Round((_totalTweets / (DateTime.Now - _startTime).TotalSeconds), 2);
                _tweetsPerMinute = Math.Round(_tweetsPerSecond * 60, 2);
                _tweetsPerHour = Math.Round(_tweetsPerMinute * 60, 2);
            }
            catch (Exception e)
            {
                _error.LogError("CalculateHelper.GetPercentOfTweetsWithObject()", e.Message, e, ErrorSeverity.LogError);
            }
        }
    }
}
