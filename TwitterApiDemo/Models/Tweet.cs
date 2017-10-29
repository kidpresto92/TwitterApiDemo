using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterApiDemo.Models
{
    public class Tweet
    {
        // JSON keys used to parse statuses
        public const String JSON_KEY_ID = "id";
        public const String JSON_KEY_MESSAGE = "text";
        public const String JSON_KEY_TIME_STAMP = "created_at";
        public const String JSON_KEY_RETWEET_COUNT = "retweet_count";
        public const String JSON_KEY_USER = "user";

        // The following are found inside the "user" JSON
        public const String JSON_KEY_NAME = "name";
        public const String JSON_KEY_SCREEN_NAME = "screen_name";
        public const String JSON_KEY_PROFILE_IMAGE_URL = "profile_image_url_https";
        public const String JSON_KEY_BACKGROUND_COLOR = "profile_background_color";

        // Model's members
        private String tweetID;
        private String name;
        private String screenName;
        private String profileImageUrl;
        private String backgroundColor;
        private String message;
        private String timeStamp;
        private int retweetCount;
        
        public Tweet (JObject statusJson)
        {
            tweetID         = (String) statusJson.GetValue(JSON_KEY_ID);
            message         = (String) statusJson.GetValue(JSON_KEY_MESSAGE);
            timeStamp       = (String) statusJson.GetValue(JSON_KEY_TIME_STAMP);
            retweetCount    = (int) statusJson.GetValue(JSON_KEY_RETWEET_COUNT);

            JObject user    = (JObject)statusJson.GetValue(JSON_KEY_USER);
            
            name            = (String) user.GetValue(JSON_KEY_NAME);
            screenName      = (String) user.GetValue(JSON_KEY_SCREEN_NAME);
            profileImageUrl = (String) user.GetValue(JSON_KEY_PROFILE_IMAGE_URL);
            backgroundColor = (String) user.GetValue(JSON_KEY_BACKGROUND_COLOR);
        }

        public override string ToString()
        {
            String output = "";
            output += "ID: " + tweetID + "\n";
            output += "Name: " + name + "\n";
            output += "screenName: " + screenName + "\n";
            output += "profileImageUrl: " + profileImageUrl + "\n";
            output += "backgroundColor: " + backgroundColor + "\n";
            output += "message: " + message + "\n";
            output += "timeStamp: " + timeStamp + "\n";
            output += "retweetCount: " + retweetCount + "\n";
            return output;
        }

        public String getLikeTweetUrl()
        {
            return "https://twitter.com/intent/like?tweet_id=" + tweetID;
        }

        // Getters
        public String getID()
        {
            return tweetID;
        }
        public String getName()
        {
            return name;
        }
        public String getScreenName()
        {
            return screenName;
        }
        public String getProfileImageUrl()
        {
            return profileImageUrl;
        }
        public String getBackgroundColor()
        {
            return backgroundColor;
        }
        public String getMessage()
        {
            return message;
        }
        public String getTimeStamp()
        {
            return timeStamp;
        }
        public int getRetweetCount()
        {
            return retweetCount;
        }
    }
}