using System;
using System.Collections.Generic;
using TwitterAPIClient.Interfaces;

namespace TwitterAPIClient.Models
{
    public class TweetStats : ITweetStats
    {
        public DateTime StartTime { get; set; }

        /// <summary>
        /// For calculating the highest occurence of an object
        /// </summary>
        public List<TweetObjects> TweetObjectList { get; set; } = new List<TweetObjects>();

        /// <summary>
        /// For calculating the percentage of one or more occurence of an object in a tweet
        /// </summary>
        public List<TweetCounts> TweetCountList { get; set; } = new List<TweetCounts>();
    }
}
