using System;
using System.Collections.Generic;
using TwitterAPIClient.Models;

namespace TwitterAPIClient.Interfaces
{
    public interface ITweetStats
    {
        DateTime StartTime { get; set; }
        List<TweetObjects> TweetObjectList { get; set; }
        List<TweetCounts> TweetCountList { get; set; }
    }
}