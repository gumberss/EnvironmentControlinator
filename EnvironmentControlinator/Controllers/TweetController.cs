using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Streaming;
using VoiceInfra;

namespace EnvironmentControlinator.Controllers
{
    public class TweetController
    {
        private static object _lock = new object();
        private static object _lockTweets = new object();

        public TweetController()
        {
            Tweets = new List<string>();
        }

        public async Task Listen()
        {
            var userClient = new TwitterClient("",
                "",
                "",
                "");

            //var user = await userClient.Users.GetAuthenticatedUserAsync();

            //var a = await userClient.Timelines.GetUserTimelineAsync("gumbbers");

            var stream = userClient.Streams.CreateFilteredStream();
            stream.AddTrack("@gumberslive");

            var badWords = await File.ReadAllLinesAsync("Datas/BadWords.txt");

            stream.MatchingTweetReceived += async (sender, eventReceived) =>
            {
                var tweet = eventReceived.Tweet;

                var tweetWords = tweet.ToString().Split();

                if (tweetWords.Any(word => badWords.Contains(word)))
                {
                    lock (_lock)
                    {
                        if (!Directory.Exists("../BadTweets"))
                            Directory.CreateDirectory("../BadTweets");

                        if (!File.Exists("../BadTweets/tweets.txt"))
                            File.Create("../BadTweets/tweets.txt");
                    }

                    await File.AppendAllTextAsync("../BadTweets/tweets", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{tweet.CreatedBy}:{tweet}");
                }
                else
                {
                    lock (_lock)
                    {
                        if (!Directory.Exists("../GoodTweets"))
                            Directory.CreateDirectory("../GoodTweets");

                        if (!File.Exists("../GoodTweets/tweets.txt"))
                            File.Create("../GoodTweets/tweets.txt");
                    }

                    await File.AppendAllTextAsync("../GoodTweets/tweets", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{tweet.CreatedBy}:{tweet}");

                    var fullTweetText = $"{tweet.CreatedBy} disse: {tweet}";

                    AddTweet(fullTweetText);

                    Task.Run(() => Synthesizer.SynthesizeAudio(fullTweetText));
                }
            };

            await stream.StartMatchingAnyConditionAsync();
        }

        private static List<String> Tweets;

        public void AddTweet(String tweet)
        {
            lock (_lockTweets)
            {
                Tweets.Add(tweet);
            }
        }

        public String GetTweet()
        {
            lock (_lockTweets)
            {
                var tweet = Tweets[0];

                Tweets.Remove(tweet);

                return tweet;
            }
        }
    }
}