using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TwitterApiDemo.Models;

namespace TwitterApiDemo.Controllers
{
    public class TwitterController : Controller
    {
        private const String tweetSearchUrl = "https://api.twitter.com/1.1/search/tweets.json";
        private const String generateTokenUrl = "https://api.twitter.com/oauth2/token";

        //Obtained by creating twitter app project: https://apps.twitter.com/
        private const String consumerKey = "ZLaclnUHdsJt17WJakIHuTKCo";
        private const String consumerSecret = "0gQoAghcRV0RMV77BRkP0ulCZydK98aaSwNQtFdd4wKVushIUS";

        private String token = null;
        private String authHeader = null;

        public ActionResult Index(String tag)
        {
            if (!String.IsNullOrEmpty(tag))
            {
                if (String.IsNullOrEmpty(token))
                {
                    getBearerToken();

                }

                //Check to confirm that the token was found
                if (!String.IsNullOrEmpty(token))
                {
                    List<Tweet> tweets = searchTweets(tag);

                    if(tweets.Count > 0)
                    {
                        ViewBag.Message = "Search Results for: " + tag;

                        ViewData["tweets"] = tweets;
                    }
                    else
                    {
                        ViewBag.Message = "Could not find tweets with: " + tag;
                    }

                }
                else
                {
                    ViewBag.Message = "Error finding authorization token";
                }

            }
            else
            {
                ViewBag.Message = "Please Search for a Hashtag in the Search Bar Above";
            }
            return View();
        }

        private void getBearerToken()
        {
            //Create Post Data
            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("grant_type", "client_credentials");
            string postdata = outgoingQueryString.ToString();

            //Create Auth Header

            //Per Twitter documentation: https://developer.twitter.com/en/docs/basics/authentication/overview/application-only
            //Concatenate the encoded consumer key, a colon character ”:”, and the encoded consumer secret into a single string.
            String basicAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(consumerKey + ":" + consumerSecret));

            //Create Post Request
            HttpWebRequest webRequest = WebRequest.Create(generateTokenUrl) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;
            webRequest.Headers.Add("Authorization", "Basic " + basicAuth);

            //Send Post data through the request
            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postdata);
            requestWriter.Close();

            //Read the response of the request
            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();

            //Clean up
            responseReader.Close();
            webRequest.GetResponse().Close();

            Console.WriteLine(responseData);

            var tokenRequestJson = JObject.Parse(responseData);

            //Per Twitter's documents, verify that the token type is "bearer"
            if (((String)tokenRequestJson.GetValue("token_type")).Equals("bearer"))
            {
                token = (String)tokenRequestJson.GetValue("access_token");

                authHeader = "Bearer " + token;
            }
        }

        private List<Tweet> searchTweets(String tag)
        {
            //Make sure we're searching by hashtags
            if (!tag.First().Equals("#"))
            {
                tag = "#" + tag;
            }

            //make the tag URL friendly
            tag = HttpUtility.UrlEncode(tag);
            String url = tweetSearchUrl + "?q=" + tag + "&result_type=popular&count=100";

            //Create the search tweets request
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.Headers.Add("Authorization", authHeader);

            //Read the response from the Twitter API
            String responseData = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseData = reader.ReadToEnd();
            }


            var responseJson = JObject.Parse(responseData);

            JArray statuses = (JArray) responseJson.GetValue("statuses");

            List<Tweet> tweets = new List<Tweet>();
            foreach(JObject status in statuses)
            {
                tweets.Add(new Tweet(status));
            }

            return tweets;
        }
    }
}