using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.Models
{
    /// <summary>
    /// This will keep count of each individual tracked object such as emoji, hashtag, URL etc.. 
    /// //To be used to determine popularity (Top Emoji, Top Hashtag, etc) of the object.
    /// </summary>
    public class TweetObjects
    {
        public TweetObjectType TweetObjType { get; set; }
        public string TweetObjValue { get; set; }
        public int TotalObjCount { get; set; }
    }
}
