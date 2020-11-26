using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using TweetSource.EventSource;
using UnityEngine;

public class TestTwitter : MonoBehaviour {
    private const string hashtagColor = "#00acee";
    private const string hashtagRegex = @"(\W)(\#[a-zA-Z0-9]+\b)";
    
    public TMP_Text NameText;
    public TMP_Text UsernameText;
    public TMP_Text PostText;
    public bool ShouldDispatch;
    public float TimeToShow = 1f;
    
    public readonly Queue<TweetInfo> TweetQueue = new Queue<TweetInfo>();
    private float timeShown;
    
    private void Start() {
        TwitterAPI1.Initialize(OnEventReceived, OnStreamDown, OnStreamUp);
        TwitterAPI1.Connect();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.I)) {
            // initialize
            TwitterAPI1.Initialize(OnEventReceived, OnStreamDown, OnStreamUp);
        }

        if (Input.GetKeyUp(KeyCode.C)) {
            TwitterAPI1.Connect();
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
        }
    }

    private void OnEventReceived(object sender, TweetEventArgs evt) {
        try {
            if (!string.IsNullOrEmpty(evt.JsonText)) {
                Debug.Log($"Received tweet\n{evt.JsonText}");
                
                var jsonObject = JObject.Parse(evt.JsonText);
                var shouldDrop = jsonObject.ContainsKey("quoted_status") || jsonObject.ContainsKey("retweeted_status");
                if (shouldDrop) {
                    Debug.Log("Dropped retweet");
                    return;
                }
                
                var truncated = jsonObject.Value<bool>("truncated");
                var text = truncated ? jsonObject["extended_tweet"].Value<string>("full_text") : jsonObject.Value<string>("text");
                var displayName = jsonObject["user"].Value<string>("name");
                var username = jsonObject["user"].Value<string>("screen_name");

                text = text.Replace("\ufe0f", "");
                displayName = displayName.Replace("\ufe0f", "");
                username = username.Replace("\ufe0f", "");

                var lastSpace = text.LastIndexOf(' ');
                text = text.Substring(0, lastSpace);
                text = HttpUtility.HtmlDecode(text);
                text = Regex.Replace(text, hashtagRegex, $@"$1<color={hashtagColor}>$2</color>");
                
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
    }
}

public struct TweetInfo {
    public string Text;
    public string Name;
    public string Username;
}