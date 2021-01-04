using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.Interfaces
{
    public interface ITweetHelper
    {
        void AddTweetObject(string strInput, TweetObjectType type);
        void AddCountObject(TweetObjectType type);
        void AddToTweetCounter(TweetObjectType objType);
    }
}