using Tweetinvi.Models.V2;

namespace TwitterAPIClient.Interfaces
{
    public interface ITweetParser
    {
        void Parse(TweetV2 tweet);
    }
}
