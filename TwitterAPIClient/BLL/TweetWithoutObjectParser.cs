using Tweetinvi.Models.V2;
using TwitterAPIClient.Interfaces;
using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.BLL
{
    public class TweetWithoutObjectParser : ITweetParser
    {
        private readonly ITweetHelper _tweetHelper;

        public TweetWithoutObjectParser(ITweetHelper tweetHelper)
        {
            _tweetHelper = tweetHelper;
        }

        public void Parse(TweetV2 tweet)
        {
            _tweetHelper.AddToTweetCounter(TweetObjectType.NoTrackingObject);
        }
    }
}
