using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class TestTwitter : MonoBehaviour {
    public TMP_Text Text;
    public bool RequestDone;
    public string RequestText;

    private async void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
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
            });*/
        }

        if (RequestDone) {
            Text.text = RequestText;
            RequestDone = false;
        }
    }
}