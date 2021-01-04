using System.Collections.Generic;
using static TwitterAPIDemo.Helpers.CalculateHelper;

namespace TwitterAPIDemo.Interfaces
{
    public interface ICalculateHelper
    {
        int GetTotalTweetCount();
        double GetTweetsPerSecond();
        double GetTweetsPerMinute();
        double GetTweetsPerHour();
        List<string> GetTopTweetObjects(int howMany, TweetObjectTypes objType);
        double GetPercentOfTweetsWithObject(TweetObjectTypes objType);
    }
}