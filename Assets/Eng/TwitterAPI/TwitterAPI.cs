using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization;
using RestSharp.Serializers.NewtonsoftJson;
using UnityEngine;
using static Token;

public class TwitterAPI {
    private const string BaseURL = "https://api.twitter.com/2/";
    private const string RulesURL = "https://api.twitter.com/2/tweets/search/stream/rules";
    private const string StreamURL = "https://api.twitter.com/2/tweets/search/stream";

    private static JArray Rules = new JArray {
        new JObject {["value"] = "#indiedev", ["tag"] = "with hashtag indiedev"}
    };

    public static async Task<string> GetRules() {
        var headers = new Dictionary<string, string> {
            {"Authorization", $"Bearer {BEARER_TOKEN}"}
        };
        var client = new RestClient(RulesURL);
        client.UseNewtonsoftJson();
        var request = new RestRequest("", Method.GET);
        request.AddHeaders(headers);
        request.UseNewtonsoftJson();

        var response = await client.ExecuteGetAsync<string>(request);
        return response.Content;
    }

    public static async Task<string> DeleteRules(JObject rules) {
        var data = new JObject {["delete"] = new JObject{["ids"] = new JArray()}};
        var idsArray = data["delete"].Value<JArray>("ids");
        foreach (var rule in rules["data"].Values<JObject>()) {
            idsArray.Add(rule["id"]);
        }
        
        var headers = new Dictionary<string, string> {
            {"Content-type", "application/json"},
            {"Authorization", $"Bearer {BEARER_TOKEN}"}
        };
        var client = new RestClient(RulesURL);
        client.UseNewtonsoftJson();
        client.AddDefaultHeaders(headers);
        var request = new RestRequest("", Method.POST, DataFormat.Json);
        request.AddHeaders(headers);
        request.UseNewtonsoftJson();
        request.AddJsonBody(data, ContentType.Json);
        var response = await client.ExecutePostAsync<string>(request);
        return response.Content;
    }

    public static async Task<string> AddRules() {
        var rulesObject = new JObject {["add"] = Rules};
        var headers = new Dictionary<string, string> {
            {"Content-type", "application/json"},
            {"Authorization", $"Bearer {BEARER_TOKEN}"}
        };
     
        var client = new RestClient(RulesURL);
        client.UseNewtonsoftJson();
        client.AddDefaultHeaders(headers);
        var request = new RestRequest("", Method.POST, DataFormat.Json);
        request.AddHeaders(headers);
        request.UseNewtonsoftJson();
        request.AddJsonBody(rulesObject, ContentType.Json);
        var response = await client.ExecutePostAsync<string>(request);
        return response.Content;
    }

    public static async void GetTweet() {
        var headers = new Dictionary<string, string> {
            {"Authorization", $"Bearer {BEARER_TOKEN}"}
        };
        var client = new RestClient(StreamURL);
        client.UseNewtonsoftJson();
        client.AddDefaultHeaders(headers);
        var request = new RestRequest("", Method.POST, DataFormat.Json);
        request.AddHeaders(headers);
        request.UseNewtonsoftJson();
        
        // request.AddQueryParameter("expansions", "author_id");
        // request.AddQueryParameter("user.fields", "name,username,verified");
        request.ResponseWriter += stream => {
            using (stream) {
                
            }
            var reader = new StreamReader(stream);
            reader.ReadToEndAsync().ContinueWith(task => {
                Debug.Log(task.Result);
            });
        };
        var response = client.DownloadData(request);
    }
}