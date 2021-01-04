using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.Models
{
    /// <summary>
    /// To track percentages of tweets with one or more of an object
    /// This will keep count of tweets which contain a tracked object such as emoji, hashtag, URL etc...
    /// </summary>
    public class TweetCounts
    {
        public TweetObjectType TweetObjType { get; set; }
        public int TotalCount { get; set; }
    }
}
