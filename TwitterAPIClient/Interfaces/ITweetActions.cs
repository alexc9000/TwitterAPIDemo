using Tweetinvi.Models.V2;

namespace TwitterAPIClient.Interfaces
{
    public interface ITweetActions
    {
        void SetStartTime();
        void ParseTweetObjects(TweetV2 tweet);
    }
}