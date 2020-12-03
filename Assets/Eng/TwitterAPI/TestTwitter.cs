using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using TweetSource.EventSource;
using UnityEngine;
using UnityEngine.Events;

public class TestTwitter : MonoBehaviour {
    private const string hashtagColor = "#00acee";
    private const string hashtagRegex = @"(\W)(\#[a-zA-Z0-9]+\b)";
    private const string atRegex = @"(@[a-zA-Z0-9_]+\b)";

    [Header("References")]
    public TMP_Text NameText;
    public TMP_Text UsernameText;
    public TMP_Text PostText;
    [Header("Settings")]
    public bool ShouldDispatch;
    public float TimeToShow = 1f;
    public List<string> Tracker;
    public UnityEvent OnTweetReceived;


    public readonly Queue<TweetInfo> TweetQueue = new Queue<TweetInfo>();
    private float timeShown;

    private void Start() {
        Debug.LogError("Open console");
        TwitterAPI1.Initialize(OnEventReceived, OnStreamDown, OnStreamUp);
        TwitterAPI1.Connect(Tracker.ToArray());
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.I)) {
            // initialize
            TwitterAPI1.Initialize(OnEventReceived, OnStreamDown, OnStreamUp);
        }

        if (Input.GetKeyUp(KeyCode.C)) {
            TwitterAPI1.Connect(Tracker.ToArray());
        }

        if (Input.GetKeyUp(KeyCode.D)) {
            TwitterAPI1.Disconnect();
        }

        if (ShouldDispatch) {
            TwitterAPI1.Dispatch();
        }

        timeShown += Time.deltaTime;
        if (TweetQueue.Count > 0 && timeShown >= TimeToShow) {
            timeShown = 0f;
            var tweet = TweetQueue.Dequeue();
            NameText.text = tweet.Name;
            UsernameText.text = tweet.Username;
            PostText.text = tweet.Text;
            OnTweetReceived?.Invoke();
        }
    }

    private void OnEventReceived(object sender, TweetEventArgs evt) {
        try {
            if (!string.IsNullOrEmpty(evt.JsonText)) {
                Debug.Log($"Received tweet\n{evt.JsonText}");

                var jsonObject = JObject.Parse(evt.JsonText);
                var isRetweet = jsonObject.ContainsKey("retweeted_status");
                var isQuotedStatus = jsonObject.ContainsKey("quoted_status");
                
                var lang = jsonObject.Value<string>("lang");
                var isEnglishDutch = lang == "en" || lang == "nl";
                if (!isEnglishDutch) {
                    Debug.Log("Dropped retweet");
                    return;
                }

                bool truncated;
                string text, displayName, username;
                if (isRetweet) {
                    truncated = jsonObject["retweeted_status"].Value<bool>("truncated");
                    text = truncated ? jsonObject["retweeted_status"]["extended_tweet"].Value<string>("full_text") : jsonObject["retweeted_status"].Value<string>("text");
                    displayName = jsonObject["retweeted_status"]["user"].Value<string>("name");
                    username = jsonObject["retweeted_status"]["user"].Value<string>("screen_name");
                } else if (isQuotedStatus) {
                    truncated = jsonObject["quoted_status"].Value<bool>("truncated");
                    text = truncated ? jsonObject["quoted_status"]["extended_tweet"].Value<string>("full_text") : jsonObject["quoted_status"].Value<string>("text");
                    displayName = jsonObject["quoted_status"]["user"].Value<string>("name");
                    username = jsonObject["quoted_status"]["user"].Value<string>("screen_name");
                } else {
                    truncated = jsonObject.Value<bool>("truncated");
                    text = truncated ? jsonObject["extended_tweet"].Value<string>("full_text") : jsonObject.Value<string>("text");
                    displayName = jsonObject["user"].Value<string>("name");
                    username = jsonObject["user"].Value<string>("screen_name");
                }

                text = text.Replace("\ufe0f", "");
                displayName = displayName.Replace("\ufe0f", "");
                username = username.Replace("\ufe0f", "");

                var lastSpace = text.LastIndexOf(' ');
                text = text.Substring(0, lastSpace);
                text = HttpUtility.HtmlDecode(text);
                text = Regex.Replace(text, hashtagRegex, $@"$1<color={hashtagColor}>$2</color>");
                text = Regex.Replace(text, atRegex, $@"<color={hashtagColor}>$1</color>");

                TweetQueue.Enqueue(new TweetInfo {Text = text, Name = displayName, Username = $"@{username}"});
            }
        } catch (JsonReaderException jex) {
            Debug.LogError("Error JSON read failed: " + jex.Message);
            Debug.LogException(jex);
        }
    }

    private void OnStreamUp(object sender, TweetEventArgs evt) {
        Debug.Log("On Stream Up");
    }

    private void OnStreamDown(object sender, TweetEventArgs evt) {
        Debug.Log($"On Stream Down {evt.InfoText}\n{evt.JsonText}");
        TwitterAPI1.Connect(Tracker.ToArray());
    }
}

public struct TweetInfo {
    public string Text;
    public string Name;
    public string Username;
}