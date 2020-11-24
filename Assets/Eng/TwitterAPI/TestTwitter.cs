using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using TweetSource.EventSource;
using UnityEngine;

public class TestTwitter : MonoBehaviour {
    public TMP_Text NameText;
    public TMP_Text UsernameText;
    public TMP_Text PostText;
    public bool ShouldDispatch;
    public readonly Queue<TweetInfo> TweetQueue = new Queue<TweetInfo>();
    public float TimeShown;
    public float TimeToShow = 1f;

    private void Start() {
        TwitterAPI1.Initialize(OnEventReceived, OnStreamDown, OnStreamUp);
        TwitterAPI1.Connect();
    }

    private void Update() {
        /*if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Sending Twitter Rules");
            TwitterAPI.GetRules().ContinueWith(task => {
                var rules = JObject.Parse(task.Result);
                if (rules.ContainsKey("data")) {
                    TwitterAPI.DeleteRules(rules).ContinueWith(task1 => {
                        Debug.Log($"Twitter Rules deleted");
                        RequestDone = true;
                        RequestText = task1.Result;
                        
                        TwitterAPI.AddRules().ContinueWith(task3 => {
                            Debug.Log($"Twitter Rules Sent");
                            RequestDone = true;
                            RequestText = task3.Result;
                        });
                    });
                } else {
                    TwitterAPI.AddRules().ContinueWith(task2 => {
                        Debug.Log($"Twitter Rules Sent");
                        RequestDone = true;
                        RequestText = task2.Result;
                    });
                }
                RequestDone = true;
                RequestText = task.Result;
            });
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            Debug.Log("Getting Twitter Rules");
            TwitterAPI.GetRules().ContinueWith(task => {
                Debug.Log($"Twitter Rules received");
                RequestDone = true;
                RequestText = task.Result;
            });
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log("Deleting Twitter Rules");
            TwitterAPI.GetRules().ContinueWith(task => {
                var rules = JObject.Parse(task.Result);
                TwitterAPI.DeleteRules(rules).ContinueWith(task1 => {
                    Debug.Log($"Twitter Rules deleted");
                    RequestDone = true;
                    RequestText = task1.Result;
                });
            });
        }
        
        if (Input.GetKeyDown(KeyCode.V)) {
            Debug.Log("Getting Tweet");
            /*TwitterAPI.GetTweet().ContinueWith(task => {
                Debug.Log($"Tweet received");
                RequestDone = true;
                RequestText = task.Result;
            });#1#
        }
        */
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

        TimeShown += Time.deltaTime;
        if (TweetQueue.Count > 0 && TimeShown >= TimeToShow) {
            TimeShown = 0f;
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