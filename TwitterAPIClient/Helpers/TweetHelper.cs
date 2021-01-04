using System.Linq;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.Helpers
{
    public class TweetHelper : ITweetHelper
    {
        private readonly ITweetStats _tweetStats;

        public TweetHelper(ITweetStats tweetStats)
        {
            _tweetStats = tweetStats;
        }

        public void AddTweetObject(string strInput, TweetObjectType type)
        {
            TweetObjects objNew = new TweetObjects
            {
                TweetObjType = type,
                TweetObjValue = strInput,
                TotalObjCount = 1
            };

            _tweetStats.TweetObjectList.Add(objNew);
        }

        public void AddCountObject(TweetObjectType type)
        {
            TweetCounts objNew = new TweetCounts
            {
                TotalCount = 1,
                TweetObjType = type
            };

            _tweetStats.TweetCountList.Add(objNew);
        }

        public void AddToTweetCounter(TweetObjectType objType)
        { 
            TweetCounts objOld = _tweetStats.TweetCountList.FirstOrDefault(x => x.TweetObjType == objType);
            if (objOld != null)
            {
                objOld.TotalCount++;
            }
            else
            {
                AddCountObject(objType);
            }
        }
    }
}
