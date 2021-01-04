using System.Collections.Generic;

namespace TwitterAPIDemo.Models
{
    public class TweetStatistics
    {
        public List<string> ErrorList { get; set; }
        public int TotalTweetCount { get; set; } = 0;
        public double TotalTweetsPerSecond { get; set; } = 0;
        public double TotalTweetsPerMinute { get; set; } = 0;
        public double TotalTweetsPerHour { get; set; } = 0;

        public List<string> TopEmojis { get; set; } = new List<string>();
        public List<string> TopHashTags { get; set; } = new List<string>();
        public List<string> TopDomains { get; set; } = new List<string>();

        public double PercentOfTweetsWithEmojis { get; set; }
        public double PercentOfTweetsWithURL { get; set; }
        public double PercentOfTweetsWithPhotoURL { get; set; }
    }
}
