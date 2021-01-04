using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi.Models.V2;
using TwitterAPIClient.BLL;
using TwitterAPIClient.Helpers;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using TwitterAPIDemo.Helpers;
using TwitterAPIDemo.Interfaces;
using static TwitterAPIDemo.Helpers.CalculateHelper;

namespace TwitterAPIDemo.Test
{
    [TestClass]
    public class APIClient_Test
    {
        private readonly IClientError _error;
        private ITweetStats _tweetStats;
        private readonly ITweetHelper _tweetHelper;
        private readonly ICalculateHelper _calculateHelper;

        public APIClient_Test()
        {
            var mockLogger = new Mock<ILogger<ClientError>>();
            _error = new ClientError(mockLogger.Object);
            _tweetStats = new TweetStats();
            _tweetStats.StartTime = DateTime.Now;
            _tweetHelper = new TweetHelper(_tweetStats);
            _calculateHelper = new CalculateHelper(_tweetStats, _error);
        }

        #region CreateTweets
        private List<TweetV2> CreateTweetList()
        {
            List<TweetV2> tweets = new List<TweetV2>();
            tweets.Add(CreateTweet("This is a test tweet."));
            tweets.Add(CreateTweet("This is a test tweet with emoji. 😘"));
            tweets.Add(CreateTweet("RT @_harrisonJNR: You deserve more.. baby 😘😘💜"));
            tweets.Add(CreateTweetWithHashtag("This is a test tweet with a #BDAY.", "#BDAY"));
            tweets.Add(CreateTweetWithHashtag("This is a test tweet with #HAPPYBIRTHDAY #BDAY.", "#HAPPYBIRTHDAY", "#BDAY"));
            tweets.Add(CreateTweetWithUrl("This is a test tweet with an URL http://www.google.com.", "http://www.google.com"));
            tweets.Add(CreateTweetWithUrl("This is a test tweet with an URL http://www.google.com http://www.facebook.com.", "http://www.google.com", "http://www.facebook.com"));
            tweets.Add(CreateTweetWithUrl("This is a test tweet with an PhotoURL https://pbs.twimg.com/semantic_core_img/1345067763455279104/ewDyghGU?format=jpg&name=small.", "https://pbs.twimg.com/semantic_core_img/1345067763455279104/ewDyghGU?format=jpg&name=small"));

            return tweets;
        }

        private TweetV2 CreateTweet(string tweetText)
        {
            //Could have emojis or just text.
            TweetV2 tweet = new TweetV2
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Urls = System.Array.Empty<UrlV2>(),
                    Hashtags = System.Array.Empty<HashtagV2>()
                }
            };

            return tweet;
        }
        private TweetV2 CreateTweetWithUrl(string tweetText, string url)
        {
            TweetV2 tweet = new TweetV2
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Urls = new UrlV2[]
                    {
                       new UrlV2{ DisplayUrl = url, ExpandedUrl = url }
                    },
                    Hashtags = System.Array.Empty<HashtagV2>()
                }
            };

            return tweet;
        }
        private TweetV2 CreateTweetWithUrl(string tweetText, string url1, string url2)
        {
            TweetV2 tweet = new TweetV2
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Urls = new UrlV2[]
                    {
                       new UrlV2{ DisplayUrl = url1, ExpandedUrl = url1 },
                       new UrlV2{ DisplayUrl = url2, ExpandedUrl = url2 }
                    },
                    Hashtags = System.Array.Empty<HashtagV2>()
                }
            };

            return tweet;
        }
        private TweetV2 CreateTweetWithHashtag(string tweetText, string hashtag)
        {
            TweetV2 tweet = new TweetV2
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Hashtags = new HashtagV2[]
                    {
                       new HashtagV2{ Hashtag = hashtag}
                    },
                    Urls = System.Array.Empty<UrlV2>()

                }
            };

            return tweet;
        }
        private TweetV2 CreateTweetWithHashtag(string tweetText, string hashtag1, string hashtag2)
        {
            TweetV2 tweet = new TweetV2
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Hashtags = new HashtagV2[]
                    {
                       new HashtagV2{ Hashtag = hashtag1},
                       new HashtagV2{ Hashtag = hashtag2}
                    },
                    Urls = System.Array.Empty<UrlV2>()

                }
            };

            return tweet;
        }
        #endregion

        [TestMethod]
        public void TotalTweetCount_Test()
        {
            int expectedResult = 8;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetActions tweetActions;
            tweetActions = new TweetActions(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                tweetActions.ParseTweetObjects(tweet);
            }

            int actualResult = (from x in _tweetStats.TweetCountList select x.TotalCount).Sum();
            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void ParseTweetsWithHashtags_Test()
        {
            //Two tweets that include hashtags are expected.
            int expectedResult = 2;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetHashTagParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                HashtagV2[] hashtags = tweet.Entities?.Hashtags;
                if (hashtags != null && hashtags.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }
            int actualResult = (from x in _tweetStats.TweetCountList where x.TweetObjType == Enums.TweetObjectType.Hashtag select x.TotalCount).Sum();
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ParseTweetsWithEmojis_Test()
        {
            //Two tweets that include emojis are expected.
            int expectedResult = 2;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetEmojiParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                MatchCollection emojis = Regex.Matches(tweet.Text, TwitterAPIClient.Constants.EmojiRegEx);
                if (emojis != null && emojis.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }
            int actualResult = (from x in _tweetStats.TweetCountList where x.TweetObjType == Enums.TweetObjectType.Emoji select x.TotalCount).Sum();
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ParseTweetsWithURLS_Test()
        {
            int expectedResult = 2;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetURLParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                UrlV2[] urls = tweet.Entities?.Urls;
                if (urls != null && urls.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }

            int actualResult = (from x in _tweetStats.TweetCountList where x.TweetObjType == Enums.TweetObjectType.Url select x.TotalCount).Sum();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ParseTweetsWithPhotoURLS_Test()
        {
            int expectedResult = 1;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetURLParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                UrlV2[] urls = tweet.Entities?.Urls;
                if (urls != null && urls.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }

            int actualResult = (from x in _tweetStats.TweetCountList where x.TweetObjType == Enums.TweetObjectType.PhotoUrl select x.TotalCount).Sum();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ParseTweetsWithNoObjects_Test()
        {
            int expectedResult = 1;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetWithoutObjectParser(_tweetHelper);

            foreach (var tweet in tweetsList)
            {
                //Just reducing the list to sort through
                UrlV2[] urls = tweet.Entities?.Urls;
                HashtagV2[] hashtags = tweet.Entities?.Hashtags;
                MatchCollection emojis = Regex.Matches(tweet.Text, TwitterAPIClient.Constants.EmojiRegEx);
                if ((urls == null || !urls.Any()) && (hashtags == null || !hashtags.Any()) && (emojis == null || !emojis.Any()))
                {
                    tweetParser.Parse(tweet);
                }
            }

            int actualResult = (from x in _tweetStats.TweetCountList where x.TweetObjType == Enums.TweetObjectType.NoTrackingObject select x.TotalCount).Sum();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TopEmoji_Test()
        {
            string expectedResult = "😘";
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetEmojiParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                MatchCollection emojis = Regex.Matches(tweet.Text, TwitterAPIClient.Constants.EmojiRegEx);
                if (emojis != null && emojis.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }

            List<string> results = _calculateHelper.GetTopTweetObjects(1, TweetObjectTypes.Emoji);
            string actualResult = results.First();

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void TopHashtag_Test()
        {
            string expectedResult = "#BDAY";
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetHashTagParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                HashtagV2[] hashtags = tweet.Entities?.Hashtags;
                if (hashtags != null && hashtags.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }

            List<string> results = _calculateHelper.GetTopTweetObjects(1, TweetObjectTypes.Hashtag);
            string actualResult = results.First();

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void TopDomain_Test()
        {
            string expectedResult = "www.google.com";
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetParser tweetParser;
            tweetParser = new TweetURLParser(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                UrlV2[] urls = tweet.Entities?.Urls;
                if (urls != null && urls.Any())
                {
                    tweetParser.Parse(tweet);
                }
            }

            List<string> results = _calculateHelper.GetTopTweetObjects(1, TweetObjectTypes.Domain);
            string actualResult = results.First();

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void PercentWithEmojis_Test()
        {
            int expectedResult = 25;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetActions tweetActions;
            tweetActions = new TweetActions(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                tweetActions.ParseTweetObjects(tweet);
            }
            _calculateHelper.GetTotalTweetCount();
            double actualResult = _calculateHelper.GetPercentOfTweetsWithObject(TweetObjectTypes.Emoji);

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void PercentWithUrls_Test()
        {
            int expectedResult = 25;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetActions tweetActions;
            tweetActions = new TweetActions(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                tweetActions.ParseTweetObjects(tweet);
            }

            _calculateHelper.GetTotalTweetCount();
            double actualResult = _calculateHelper.GetPercentOfTweetsWithObject(TweetObjectTypes.Url);

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void PercentWithPhotoUrls_Test()
        {
            double expectedResult = 12.5;
            List<TweetV2> tweetsList = CreateTweetList();
            ITweetActions tweetActions;
            tweetActions = new TweetActions(_tweetHelper, _tweetStats, _error);

            foreach (var tweet in tweetsList)
            {
                tweetActions.ParseTweetObjects(tweet);
            }

            _calculateHelper.GetTotalTweetCount();
            double actualResult = _calculateHelper.GetPercentOfTweetsWithObject(TweetObjectTypes.PhotoUrl);

            Assert.AreEqual(expectedResult, actualResult);

        }
    }
}
