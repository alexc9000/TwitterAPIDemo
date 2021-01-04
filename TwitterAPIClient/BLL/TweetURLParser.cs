using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi.Models.V2;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using static TwitterAPIClient.Models.Enums;

namespace TwitterAPIClient.BLL
{
    public class TweetURLParser : ITweetParser
    {
        private readonly ITweetHelper _tweetHelper;
        private readonly ITweetStats _tweetStats;
        private readonly IClientError _error;

        public TweetURLParser(ITweetHelper tweetHelper, ITweetStats tweetStats, IClientError error)
        {
            _tweetHelper = tweetHelper;
            _tweetStats = tweetStats;
            _error = error;
        }

        public void Parse(TweetV2 tweet)
        {
            int urlCount = 0;
            int photoUrlCount = 0;
            UrlV2[] urls = tweet.Entities?.Urls;
            foreach (var url in urls)
            {
                ParseDomain(url);

                if (IsPhotoUrl(url))
                {
                    ParsePhotoUrl(url);
                    photoUrlCount++;
                }
                else
                {
                    ParseUrl(url);
                    urlCount++;
                }
            }

            if (urlCount > 0)
            {
                _tweetHelper.AddToTweetCounter(TweetObjectType.Url);
            }
            if (photoUrlCount > 0)
            {
                _tweetHelper.AddToTweetCounter(TweetObjectType.PhotoUrl);
            }
        }

        private void ParseUrl(UrlV2 url)
        {
            try
            {
                if (_tweetStats.TweetObjectList != null)
                {
                    TweetObjects objOld = _tweetStats.TweetObjectList
                        .Where(c => c.TweetObjType == TweetObjectType.Url)
                        .FirstOrDefault(x => x.TweetObjValue == url.ExpandedUrl);
                    if (objOld != null)
                    {
                        objOld.TotalObjCount++;
                    }
                    else
                    {
                        _tweetHelper.AddTweetObject(url.ExpandedUrl, TweetObjectType.Url);
                    }
                }
                else
                {
                    _tweetHelper.AddTweetObject(url.ExpandedUrl, TweetObjectType.Url);
                }
            }
            catch (Exception e)
            {
                _error.LogError("TweetURLParser.ParseUrl()", e.Message, e, ErrorSeverity.LogError);
            }
        }

        private void ParsePhotoUrl(UrlV2 url)
        {
            try
            {
                if (_tweetStats.TweetObjectList != null)
                {
                    TweetObjects objOld = _tweetStats.TweetObjectList
                        .Where(c => c.TweetObjType == TweetObjectType.PhotoUrl)
                        .FirstOrDefault(x => x.TweetObjValue == url.ExpandedUrl);
                    if (objOld != null)
                    {
                        objOld.TotalObjCount++;
                    }
                    else
                    {
                        _tweetHelper.AddTweetObject(url.ExpandedUrl, TweetObjectType.PhotoUrl);
                    }
                }
                else
                {
                    _tweetHelper.AddTweetObject(url.ExpandedUrl, TweetObjectType.PhotoUrl);
                }
            }
            catch (Exception e)
            {
                _error.LogError("TweetURLParser.ParsePhotoUrl()", e.Message, e, ErrorSeverity.LogError);
            }
        }

        private bool IsPhotoUrl(UrlV2 url)
        {
            bool result = false;
            List<string> photoURLs = new List<string>()
            {
                "pbs.twimg.com", //added this one
                "pic.instagram.com" //this may be obsolete
            };

            //Here's another matching-style since the provided keywords didn't produce results.
            Match m = Regex.Match(url.ExpandedUrl, @"https://twitter.com/(.+)/photo/1", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = true;
            }
            else
            {
                foreach (string photoURL in photoURLs)
                {
                    if (url.ExpandedUrl.Contains(photoURL)) result = true;
                }
            }

            return result;
        }

        private void ParseDomain(UrlV2 url)
        {
            try
            {
                if (_tweetStats.TweetObjectList != null)
                {
                    TweetObjects objOld = _tweetStats.TweetObjectList
                        .Where(c => c.TweetObjType == TweetObjectType.Domain)
                        .FirstOrDefault(x => x.TweetObjValue == ExtractDomain(url));
                    if (objOld != null)
                    {
                        objOld.TotalObjCount++;
                    }
                    else
                    {
                        _tweetHelper.AddTweetObject(ExtractDomain(url), TweetObjectType.Domain);
                    }
                }
                else
                {
                    _tweetHelper.AddTweetObject(ExtractDomain(url), TweetObjectType.Domain);
                }
            }
            catch (Exception e)
            {
                _error.LogError("TweetURLParser.ParseDomain()", e.Message, e, ErrorSeverity.LogError);
            }
        }

        private string ExtractDomain(UrlV2 url)
        {
            Uri myUri = new Uri(url.ExpandedUrl);
            return myUri.Host;
        }
    }
}
